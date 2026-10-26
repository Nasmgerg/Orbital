using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orbital.Context;
using Xunit;
using Xunit.Sdk;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace EvaluatorTests;

public sealed class SubmissionTests
{
    private static readonly Assembly App = typeof(CrewContext).Assembly;

    private static Type RequireType(string fullName) => App.GetType(fullName)
        ?? throw new XunitException($"Hiányzik a(z) {fullName} osztály. Ellenőrizd a névteret és az osztály nevét!");

    private static PropertyInfo RequireProperty(Type type, string name, Type propertyType)
    {
        var property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
        Assert.True(property != null, $"{type.Name}: hiányzik a publikus '{name}' property.");
        Assert.True(property!.PropertyType == propertyType,
            $"{type.Name}.{name}: elvárt típus: {propertyType.Name}, tényleges: {property.PropertyType.Name}.");
        Assert.True(property.GetMethod?.IsPublic == true && property.SetMethod?.IsPublic == true,
            $"{type.Name}.{name}: publikus get és set szükséges.");
        return property;
    }

    private static Control RequireControl(Control root, string name, Type expected)
    {
        var control = root.Controls.Find(name, true).FirstOrDefault();
        Assert.True(control != null, $"Hiányzik a(z) '{name}' nevű vezérlő a(z) {root.GetType().Name} ablakról.");
        Assert.True(expected.IsInstanceOfType(control),
            $"A(z) '{name}' vezérlő típusa nem megfelelő. Elvárt: {expected.Name}; tényleges: {control!.GetType().Name}.");
        return control!;
    }

    private static T Control<T>(Form form, string name) where T : Control =>
        (T)RequireControl(form, name, typeof(T));

    private static object CreateMember(string name, string role, string sector, string specialty, int shift)
    {
        var type = RequireType("Orbital.Model.CrewMember");
        var member = Activator.CreateInstance(type) ?? throw new XunitException("A CrewMember nem példányosítható.");
        foreach (var (key, value) in new (string, object)[] {
            ("Name", name), ("Role", role), ("Sector", sector), ("Specialty", specialty), ("Shift", shift) })
        {
            var prop = type.GetProperty(key) ?? throw new XunitException($"Hiányzik a CrewMember.{key} property.");
            prop.SetValue(member, value);
        }
        return member;
    }

    // Egyetlen teszt sem írja a hallgató DB/crew.db adatbázisát.
    // A memória-adatbázis addig él, amíg ez a tesztpéldány használja.
    private sealed class TestDatabase : IDisposable
    {
        private readonly SqliteConnection connection = new("Data Source=:memory:");
        public CrewContext Context { get; }
        public ServiceProvider Services { get; }

        public TestDatabase()
        {
            connection.Open();
            var options = new DbContextOptionsBuilder<CrewContext>().UseSqlite(connection).Options;
            Context = new CrewContext(options);
            Context.Database.EnsureCreated();
            Services = new ServiceCollection().AddSingleton(Context).BuildServiceProvider();
        }

        public void Seed(params (string Name, string Role, string Sector, string Specialty, int Shift)[] rows)
        {
            foreach (var r in rows) Context.Add(CreateMember(r.Name, r.Role, r.Sector, r.Specialty, r.Shift));
            Context.SaveChanges();
        }

        public void Dispose()
        {
            Services.Dispose();
            Context.Dispose();
            connection.Dispose();
        }
    }

    private static Form NewForm(string simpleName, IServiceProvider provider)
    {
        var type = RequireType("Orbital.View." + simpleName);
        Assert.True(typeof(Form).IsAssignableFrom(type), $"{type.Name} nem Windows Form típusú.");
        var ctor = type.GetConstructor(new[] { typeof(IServiceProvider) });
        Assert.True(ctor != null, $"{type.Name}: hiányzik a publikus {type.Name}(IServiceProvider) konstruktor.");
        try { return (Form)ctor!.Invoke(new object[] { provider }); }
        catch (TargetInvocationException ex)
        {
            throw new XunitException($"A(z) {type.Name} konstruktor hibát dobott: {ex.InnerException?.Message ?? ex.Message}");
        }
    }

    private static void Click(Button button)
    {
        // A PerformClick néha csak látható Formon aktiválható; Show biztosítja a feltételeket.
        button.PerformClick();
    }

    private static void Select(ComboBox combo, string value)
    {
        var index = combo.Items.Cast<object>().ToList().FindIndex(x => x?.ToString() == value);
        Assert.True(index >= 0, $"A(z) '{combo.Name}' ComboBox nem tartalmazza ezt a lehetőséget: '{value}'.");
        combo.SelectedIndex = index;
    }

    private static void Fill(Form form, string name, string role, string? sector, string specialty, int shift)
    {
        Control<TextBox>(form, "nameTextBox").Text = name;
        Select(Control<ComboBox>(form, "roleComboBox"), role);
        var sectorCombo = Control<ComboBox>(form, "sectorComboBox");
        if (sector == null) sectorCombo.SelectedIndex = -1;
        else Select(sectorCombo, sector);
        Control<TextBox>(form, "specialtyTextBox").Text = specialty;
        Control<NumericUpDown>(form, "shiftUpDown").Value = shift;
    }

    private static Dictionary<string, object?>? ReadSaved(TestDatabase db)
    {
        var conn = db.Context.Database.GetDbConnection();
        using var command = conn.CreateCommand();
        command.CommandText = "SELECT ID, Name, Role, Sector, Specialty, Shift FROM CrewMembers LIMIT 1";
        bool opened = conn.State != System.Data.ConnectionState.Open;
        if (opened) conn.Open();
        try
        {
            using var reader = command.ExecuteReader();
            if (!reader.Read()) return null;
            return Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, i => reader.GetValue(i) as object);
        }
        finally { if (opened) conn.Close(); }
    }

    private static object? Get(object entity, string property) => entity is Dictionary<string, object?> dict ? dict.GetValueOrDefault(property) : entity.GetType().GetProperty(property)?.GetValue(entity);

    private static int Count(TestDatabase database) => database.Context.Database.SqlQueryRaw<int>("SELECT COUNT(*) AS Value FROM CrewMembers").Single();

    [Fact]
    public void T01_CrewMember_PublicPropertiesExistWithCorrectTypes()
    {
        var type = RequireType("Orbital.Model.CrewMember");
        Assert.True(type.IsPublic, "A CrewMember osztálynak public láthatóságúnak kell lennie.");
        foreach (var (name, expected) in new[] {
            ("ID", typeof(int)), ("Name", typeof(string)), ("Role", typeof(string)),
            ("Sector", typeof(string)), ("Specialty", typeof(string)), ("Shift", typeof(int)) })
            RequireProperty(type, name, expected);
    }

    [Fact]
    public void T02_CrewContext_HasCorrectDbSet()
    {
        RequireProperty(typeof(CrewContext), "CrewMembers", typeof(DbSet<>).MakeGenericType(RequireType("Orbital.Model.CrewMember")));
    }

    [Fact]
    public void T03_CrewContext_CanCreateDatabaseAndStoreEntity()
    {
        using var db = new TestDatabase();
        db.Seed(("Teszt Elek", "kutató", "Labor", "Robotika", 2));
        var saved = ReadSaved(db);
        Assert.True(saved != null, "Az EF Core nem mentette vagy nem tudta visszaolvasni a személyt.");
        Assert.True(Convert.ToInt32(Get(saved!, "ID")) > 0, "Az adatbázis nem generált pozitív ID azonosítót.");
        Assert.True(Get(saved, "Name")?.ToString() == "Teszt Elek" && Convert.ToInt32(Get(saved, "Shift")) == 2 && Get(saved, "Sector")?.ToString() == "Labor",
            "A mentett entitás adatai eltérnek a visszaolvasott rekordtól.");
    }

    [Fact]
    public void T04_CrewContext_HasCodeFirstMigration()
    {
        using var db = new TestDatabase();
        var migrations = db.Context.Database.GetMigrations().ToList();
        Assert.True(migrations.Count > 0, "Nem található EF Core Code-First migráció. Készíts Add-Migration parancs segítségével migrációt!");
    }

    [Fact]
    public void T05_Forms_HaveServiceProviderConstructors()
    {
        foreach (var name in new[] { "StartUp", "Add" })
        {
            var type = RequireType("Orbital.View." + name);
            Assert.True(type.GetConstructor(new[] { typeof(IServiceProvider) }) != null,
                $"A(z) {name} Formhoz publikus IServiceProvider paraméteres konstruktor szükséges.");
        }
    }

    [StaFact]
    public void T06_AddForm_RequiredControlsAndErrorProviderExist()
    {
        using var db = new TestDatabase();
        using var form = NewForm("Add", db.Services);
        foreach (var (name, type) in new[] {
            ("nameTextBox", typeof(TextBox)), ("roleComboBox", typeof(ComboBox)),
            ("sectorComboBox", typeof(ComboBox)), ("specialtyTextBox", typeof(TextBox)),
            ("shiftUpDown", typeof(NumericUpDown)), ("addButton", typeof(Button)),
            ("cancelButton", typeof(Button)) }) RequireControl(form, name, type);
        var field = form.GetType().GetField("errorProvider", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        Assert.True(field != null && typeof(ErrorProvider).IsAssignableFrom(field.FieldType),
            "Az Add Formból hiányzik a 'errorProvider' nevű ErrorProvider mező.");
    }

    [StaFact]
    public void T07_AddForm_ComboBoxesContainRequiredValues()
    {
        using var db = new TestDatabase();
        using var form = NewForm("Add", db.Services);
        var role = Control<ComboBox>(form, "roleComboBox");
        var sector = Control<ComboBox>(form, "sectorComboBox");
        Assert.True(role.DropDownStyle == ComboBoxStyle.DropDownList,
            "A roleComboBox DropDownStyle értéke legyen DropDownList.");
        Assert.True(sector.DropDownStyle == ComboBoxStyle.DropDownList,
            "A sectorComboBox DropDownStyle értéke legyen DropDownList.");
        foreach (var item in new[] { "kutató", "technikus", "egyéb" }) Select(role, item);
        foreach (var item in new[] { "Labor", "Irányítás", "Műszaki", "Orvosi" }) Select(sector, item);
    }

    [StaFact]
    public void T08_AddForm_ShiftRangeIsOneToThree()
    {
        using var db = new TestDatabase();
        using var form = NewForm("Add", db.Services);
        var shift = Control<NumericUpDown>(form, "shiftUpDown");
        Assert.True(shift.Minimum == 1 && shift.Maximum == 3,
            $"A shiftUpDown megengedett tartománya 1–3. Tényleges: {shift.Minimum}–{shift.Maximum}.");
    }

    [StaFact]
    public void T09_AddForm_ValidResearcherIsSavedToDatabase()
    {
        using var db = new TestDatabase();
        using var form = NewForm("Add", db.Services);
        form.Show();
        Fill(form, "  Ada Lovelace  ", "kutató", "Labor", "Számítástechnika", 3);
        Click(Control<Button>(form, "addButton"));
        Assert.True(form.DialogResult == DialogResult.OK, "Sikeres mentéskor a DialogResult legyen OK.");
        var saved = ReadSaved(db);
        Assert.True(saved != null, "A Mentés gomb nem hozott létre rekordot a SQLite-adatbázisban.");
        Assert.True(Get(saved!, "Name")?.ToString() == "Ada Lovelace", "A Name mezőből le kell vágni a vezető és záró szóközöket.");
        Assert.True(Get(saved, "Role")?.ToString() == "kutató" && Get(saved, "Sector")?.ToString() == "Labor" && Get(saved, "Specialty")?.ToString() == "Számítástechnika" && Convert.ToInt32(Get(saved, "Shift")) == 3,
            "A mentett személy Role, Sector, Specialty vagy Shift mezője helytelen.");
    }

    [StaFact]
    public void T10_AddForm_TechnicianWithoutSectorIsSavedWithZeroShift()
    {
        using var db = new TestDatabase();
        using var form = NewForm("Add", db.Services);
        form.Show();
        Fill(form, "Bela", "technikus", null, "Hajtómű", 2);
        Click(Control<Button>(form, "addButton"));
        Assert.True(form.DialogResult == DialogResult.OK, "Technikusnál a Sector opcionális: az érvényes adatot menteni kell.");
        var saved = ReadSaved(db);
        Assert.True(saved != null, "A technikus adatai nem kerültek az adatbázisba.");
        Assert.True(Convert.ToInt32(Get(saved!, "Shift")) == 0, $"Nem kutatónál a Shift legyen 0, jelenleg {Get(saved, "Shift")}.");
        Assert.True(string.IsNullOrEmpty(Get(saved, "Sector")?.ToString()), "Ha nincs kiválasztott szektor, az opcionális Sector üres legyen.");
    }

    [StaFact]
    public void T11_AddForm_OtherRoleAllowsEmptyOptionalFields()
    {
        using var db = new TestDatabase();
        using var form = NewForm("Add", db.Services);
        form.Show();
        Fill(form, "Cecil", "egyéb", null, "", 3);
        Click(Control<Button>(form, "addButton"));
        Assert.True(form.DialogResult == DialogResult.OK, "'egyéb' szerepkör esetén a Sector és Specialty nem kötelező.");
        var saved = ReadSaved(db);
        Assert.True(saved != null && Convert.ToInt32(Get(saved!, "Shift")) == 0, "'egyéb' szerepkörnél új rekordnak kell létrejönnie 0 Shift értékkel.");
    }

    [StaFact]
    public void T12_AddForm_EmptyNamePreventsSaveAndShowsError()
    {
        using var db = new TestDatabase();
        using var form = NewForm("Add", db.Services);
        form.Show();
        Fill(form, "", "kutató", "Labor", "", 1);
        Click(Control<Button>(form, "addButton"));
        Assert.True(Count(db) == 0, "Üres névvel nem szabad rekordot menteni.");
        var error = (ErrorProvider)form.GetType().GetField("errorProvider", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(form)!;
        Assert.True(!string.IsNullOrWhiteSpace(error.GetError(Control<TextBox>(form, "nameTextBox"))),
            "Üres névnél a nameTextBox mezőre ErrorProvider hibaüzenetet kell beállítani.");
    }

    [StaFact]
    public void T13_AddForm_WhitespaceNamePreventsSave()
    {
        using var db = new TestDatabase();
        using var form = NewForm("Add", db.Services);
        form.Show();
        Fill(form, "   ", "kutató", "Labor", "", 1);
        Click(Control<Button>(form, "addButton"));
        Assert.True(Count(db) == 0, "A csak szóközöket tartalmazó név nem érvényes: ne történjen mentés.");
    }

    [StaFact]
    public void T14_AddForm_ResearcherWithoutSectorPreventsSaveAndShowsError()
    {
        using var db = new TestDatabase();
        using var form = NewForm("Add", db.Services);
        form.Show();
        Fill(form, "Dora", "kutató", null, "", 2);
        Click(Control<Button>(form, "addButton"));
        Assert.True(Count(db) == 0, "Kutató szerepkörnél kötelező Sector kiválasztása; nem szabad menteni.");
        var error = (ErrorProvider)form.GetType().GetField("errorProvider", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(form)!;
        Assert.True(!string.IsNullOrWhiteSpace(error.GetError(Control<ComboBox>(form, "sectorComboBox"))),
            "Hiányzó kutatói szektornál a sectorComboBox mezőre ErrorProvider hibaüzenet kell.");
    }

    [StaFact]
    public void T15_AddForm_CancelReturnsCancelWithoutSave()
    {
        using var db = new TestDatabase();
        using var form = NewForm("Add", db.Services);
        form.Show();
        Fill(form, "Dora", "kutató", "Labor", "", 1);
        Click(Control<Button>(form, "cancelButton"));
        Assert.True(form.DialogResult == DialogResult.Cancel,
            "A Mégse gomb után a DialogResult legyen Cancel.");
        Assert.True(Count(db) == 0, "A Mégse gomb megnyomásakor nem történhet adatbázis-mentés.");
    }

    [StaFact]
    public void T16_StartUp_ControlsAndTitleAreCorrect()
    {
        using var db = new TestDatabase();
        using var form = NewForm("StartUp", db.Services);
        Control<DataGridView>(form, "crewMemberGridView");
        Assert.True(Control<Button>(form, "listButton").Text == "Listázás", "A listButton szövege legyen 'Listázás'.");
        Assert.True(Control<Button>(form, "addButton").Text == "Hozzáadás", "A StartUp addButton szövege legyen 'Hozzáadás'.");
        Assert.True(form.Text == "Orbitális Személyzeti Rendszer", "Az indítóablak címe legyen 'Orbitális Személyzeti Rendszer'.");
    }

    [StaFact]
    public void T17_StartUp_LoadsRowsAtStartupAndHidesIdByProjection()
    {
        using var db = new TestDatabase();
        db.Seed(("Kata", "kutató", "Labor", "Biológia", 1));
        using var form = NewForm("StartUp", db.Services);
        var grid = Control<DataGridView>(form, "crewMemberGridView");
        Assert.True(grid.Rows.Cast<DataGridViewRow>().Count(r => !r.IsNewRow) == 1,
            "Induláskor automatikusan listázni kell az adatbázisban már meglévő személyeket.");
        Assert.True(!grid.Columns.Contains("ID"),
            "Az ID nem szerepelhet a DataSource-ban: LINQ Select projekcióval hagyd el (ne oszlopelrejtéssel). ");
        Assert.True(grid.Columns.Contains("Name") && grid.Columns.Contains("Role"),
            "A DataGridView listájában szerepelnie kell a Name és Role oszlopoknak.");
        Assert.True(grid.ReadOnly, "A crewMemberGridView.ReadOnly értéke legyen true.");
    }

    [StaFact]
    public void T18_StartUp_OrdersByRoleThenShiftThenSector()
    {
        using var db = new TestDatabase();
        db.Seed(
            ("Zed", "technikus", "Orvosi", "", 0),
            ("Beta", "kutató", "Műszaki", "", 1),
            ("Delta", "kutató", "Labor", "", 3),
            ("Alpha", "kutató", "Orvosi", "", 1),
            ("Gamma", "kutató", "Labor", "", 1));
        using var form = NewForm("StartUp", db.Services);
        var grid = Control<DataGridView>(form, "crewMemberGridView");
        var names = grid.Rows.Cast<DataGridViewRow>().Where(r => !r.IsNewRow)
            .Select(r => r.Cells["Name"].Value?.ToString()).ToArray();
        var expected = new[] { "Gamma", "Beta", "Alpha", "Delta", "Zed" };
        Assert.True(names.SequenceEqual(expected),
            $"Rendezési hiba. Elvárt: {string.Join(", ", expected)}. Kapott: {string.Join(", ", names)}. Sorrend: Role, Shift, Sector.");
    }

    [StaFact]
    public void T19_StartUp_ListButtonRefreshesAfterNewDatabaseRecord()
    {
        using var db = new TestDatabase();
        db.Seed(("Elso", "technikus", "", "", 0));
        using var form = NewForm("StartUp", db.Services);
        form.Show();
        db.Seed(("Masodik", "kutató", "Labor", "", 1));
        Click(Control<Button>(form, "listButton"));
        var grid = Control<DataGridView>(form, "crewMemberGridView");
        var names = grid.Rows.Cast<DataGridViewRow>().Where(r => !r.IsNewRow)
            .Select(r => r.Cells["Name"].Value?.ToString()).ToArray();
        Assert.True(names.Length == 2 && names.Contains("Masodik"),
            "A Listázás gomb nem frissítette a DataGridView rekordjait az új adatbázis-tartalom alapján.");
    }
}
