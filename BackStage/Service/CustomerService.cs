using BackStage.Dto;

namespace BackStage.Service
{
    public class CustomerService
    {
        private readonly ILogger<CustomerService> _log;
        private readonly DbService _dbService;

        public CustomerService(DbService dbService, ILogger<CustomerService> log) 
        {
            _dbService = dbService;
            _log = log;
        }

        public async Task AddBlackList(AddBlackListDto dto)
        {
            await _dbService.AddBlackList(dto);
        }

        public async Task DeleteBlackList(DeleteBlackListDto dto)
        {
            await _dbService.DeleteBlackList(dto);
        }
    }
}
