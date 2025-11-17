using Api.Dto;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly MyDbContext _context;

        public RolesController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        {
            var roles = await _context.Roles
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    IsDeleted = r.IsDeleted
                })
                .ToListAsync();

            return Ok(roles);
        }

        // GET: api/Roles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetRole(Guid id)
        {
            var role = await _context.Roles
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    IsDeleted = r.IsDeleted
                })
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
            {
                return NotFound();
            }

            return Ok(role);
        }

        // POST: api/Roles
        [HttpPost]
        public async Task<ActionResult<RoleDto>> PostRole(CreateRoleDto createRoleDto)
        {
            var role = new Role
            {
                Name = createRoleDto.Name,
                Description = createRoleDto.Description
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            var roleDto = new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsDeleted = role.IsDeleted
            };

            return CreatedAtAction(nameof(GetRole), new { id = role.Id }, roleDto);
        }

        // PUT: api/Roles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(Guid id, UpdateRoleDto updateRoleDto)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            role.Name = updateRoleDto.Name ?? role.Name;
            role.Description = updateRoleDto.Description ?? role.Description;
            role.IsDeleted = updateRoleDto.IsDeleted ?? role.IsDeleted;

            _context.Entry(role).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoleExists(Guid id)
        {
            return _context.Roles.Any(e => e.Id == id);
        }
    }
}
