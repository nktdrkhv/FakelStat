using PetaPoco;

namespace FakelStat.Helpers;

public static class PetaPocoHelper
{
    public static void ConfigureMapper(ConventionMapper mapper)
    {
        mapper.ToDbConverter =
                sourceProperty =>
                    sourceProperty.PropertyType.Equals(typeof(DateOnly))
                         ? obj => ((DateOnly)obj).ToString("O")
                         : obj => obj;
        mapper.FromDbConverter =
            (targetProperty, sourceType) =>
               targetProperty.PropertyType.Name switch
               {
                   nameof(DateOnly) => obj => DateOnly.FromDateTime(DateTime.Parse(obj.ToString()!)),
                   nameof(TimeOnly) => obj => TimeOnly.FromDateTime(DateTime.Parse(obj.ToString()!)),
                   _ => obj => Convert.ChangeType(obj, targetProperty.PropertyType),
               };
    }
}