using Orbital.Context;
using Microsoft.Extensions.DependencyInjection;

namespace Orbital.View
{
    public partial class StartUp : Form
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly CrewContext _context;

        public StartUp(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
            _context = _serviceProvider.GetRequiredService<CrewContext>();
            LoadCrewMembers();
        }

        private void LoadCrewMembers()
        {
            crewMemberGridView.DataSource = _context.CrewMembers
                .OrderBy(c => c.Role)
                .ThenBy(c => c.Shift)
                .ThenBy(c => c.Sector)
                .Select(c => new
                {
                    c.Name,
                    c.Role,
                    c.Sector,
                    c.Specialty,
                    c.Shift
                })
                .ToList();
        }

        private void ListCrewMembers(object? sender, EventArgs e)
        {
            LoadCrewMembers();
        }

        private void AddCrewMember(object? sender, EventArgs e)
        {
            using var addForm = _serviceProvider.GetRequiredService<Add>();
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                LoadCrewMembers();
            }
        }
    }
}
