using BackStage.Dto;

namespace BackStage.Service
{
    public class CustomerService
    {
        private readonly DbService _dbService;
        public CustomerService(DbService dbService) 
        {
            _dbService = dbService;
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
