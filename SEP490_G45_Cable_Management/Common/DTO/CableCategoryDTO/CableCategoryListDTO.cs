namespace Common.DTO.CableCategoryDTO
{
    public class CableCategoryListDTO : CableCategoryCreateUpdateDTO
    {
        public int CableCategoryId { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }
            CableCategoryListDTO DTO = (CableCategoryListDTO)obj;
            return DTO.CableCategoryName.Equals(CableCategoryName) && DTO.CableCategoryId == CableCategoryId;
        }

        public override int GetHashCode()
        {
            return (CableCategoryId, CableCategoryName).GetHashCode();
        }
    }
}
