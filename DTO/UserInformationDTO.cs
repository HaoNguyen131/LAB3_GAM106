namespace ServerGame106.DTO
{
    public class UserInformationDTO
    {
        public string UserID { get; set; }
        public string Name { get; set; }
        public int RegionID { get; set; }
        public IFormFile Avatar { get; set; }
    }
}
