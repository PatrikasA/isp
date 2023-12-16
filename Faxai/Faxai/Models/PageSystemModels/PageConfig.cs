using Faxai.Helper;
using System.Reflection.Metadata;

public class PageConfig
{
    private string DataBaseName = "Puslapio_Nustatymai";
    public int ID { get; set; }
    public string Code { get; set; }
    public string Value { get; set; }
    public string Description { get; set; }
    public PageConfig() 
    {
        ID = 0;
        Code = string.Empty;
        Value = string.Empty;
        Description = string.Empty;
    }

    public bool SaveToDataBase()
    {
        return DataSource.UpdateData(DataBaseName+Constants.SQLInset,
            new[]
            {
                "@Kodas",Code,
                "@Reiksme", Value,
                "@Aprasymas", Description
            }
            );
    }
    public bool DeleteFromDataBase()
    {
        return DataSource.UpdateData(DataBaseName + Constants.SQLDelete,
            new[]
            {
                "@ID",ID.ToString(),
            }
            );
    }
}