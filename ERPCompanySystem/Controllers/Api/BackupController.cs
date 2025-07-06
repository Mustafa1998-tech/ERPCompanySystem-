using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ERPCompanySystem.Data;
using ERPCompanySystem.Models;
using ERPCompanySystem.Services;
using ERPCompanySystem.Attributes;
using System.Threading.Tasks;

namespace ERPCompanySystem.Controllers.Api
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class BackupController : ControllerBase
    {
        private readonly IBackupService _backupService;
        private readonly ILogger<BackupController> _logger;

        public BackupController(IBackupService backupService, ILogger<BackupController> logger)
        {
            _backupService = backupService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetBackups()
        {
            try
            {
                var backups = await _backupService.GetBackupsAsync();
                return Ok(backups);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting backups");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBackup(int id)
        {
            try
            {
                var backup = await _backupService.GetBackupByIdAsync(id);
                if (backup == null) return NotFound();
                return Ok(backup);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting backup {id}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [CustomAuthorize.RequireJwt]
        public async Task<IActionResult> CreateBackup([FromBody] BackupType type, 
            [FromQuery] string description = "")
        {
            try
            {
                var backup = await _backupService.CreateBackupAsync(type, description);
                return CreatedAtAction(nameof(GetBackup), new { id = backup.BackupId }, backup);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating backup");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("restore/{id}")]
        [CustomAuthorize.RequireJwt]
        public async Task<IActionResult> RestoreBackup(int id, 
            [FromQuery] string description = "")
        {
            try
            {
                var success = await _backupService.RestoreBackupAsync(id, description);
                if (!success) return BadRequest("Failed to restore backup");
                return Ok(new { message = "Backup restored successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error restoring backup {id}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("validate/{id}")]
        public async Task<IActionResult> ValidateBackup(int id)
        {
            try
            {
                var isValid = await _backupService.ValidateBackupAsync(id);
                return Ok(new { isValid });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error validating backup {id}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("test/{id}")]
        public async Task<IActionResult> TestRestore(int id)
        {
            try
            {
                var canRestore = await _backupService.TestRestoreAsync(id);
                return Ok(new { canRestore });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error testing backup {id}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [CustomAuthorize.RequireJwt]
        public async Task<IActionResult> DeleteBackup(int id)
        {
            try
            {
                var success = await _backupService.DeleteBackupAsync(id);
                if (!success) return BadRequest("Failed to delete backup");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting backup {id}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
