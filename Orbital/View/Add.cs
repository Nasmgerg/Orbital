using Microsoft.Extensions.DependencyInjection;
using Orbital.Context;
using Orbital.Model;

namespace Orbital.View
{
    public partial class Add : Form
    {
        private const string ResearcherRole = "kutató";

        private readonly IServiceProvider _serviceProvider;
        private readonly CrewContext _context;

        public Add(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
            _context = _serviceProvider.GetRequiredService<CrewContext>();
        }

        private void Save(object? sender, EventArgs e)
        {
            errorProvider.Clear();

            string name = nameTextBox.Text.Trim();
            string? role = roleComboBox.SelectedItem?.ToString();
            string? sector = sectorComboBox.SelectedItem?.ToString();
            bool valid = true;

            if (string.IsNullOrWhiteSpace(name))
            {
                errorProvider.SetError(nameTextBox, "A név megadása kötelező.");
                valid = false;
            }

            if (role == null)
            {
                errorProvider.SetError(roleComboBox, "A szerepkör kiválasztása kötelező.");
                valid = false;
            }
            else if (role == ResearcherRole && sector == null)
            {
                errorProvider.SetError(sectorComboBox, "Kutatónál a szektor kiválasztása kötelező.");
                valid = false;
            }

            if (!valid)
            {
                return;
            }

            var member = new CrewMember
            {
                Name = name,
                Role = role!,
                Sector = sector ?? string.Empty,
                Specialty = specialtyTextBox.Text.Trim(),
                Shift = role == ResearcherRole ? (int)shiftUpDown.Value : 0
            };

            _context.CrewMembers.Add(member);
            _context.SaveChanges();

            DialogResult = DialogResult.OK;
        }

        private void Cancel(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
