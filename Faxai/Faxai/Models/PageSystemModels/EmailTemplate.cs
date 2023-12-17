using Faxai.Helper;
using Microsoft.Extensions.Logging.Abstractions;
using System.Data;

public class EmailTemplate
{
    private string DataBaseName = "Laisko_Sablonas";
    public int ID { get; set; }
    public string Code { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public string Description { get; set; }
    public string From { get; set; }
    public EmailTemplate()
    {
        ID = 0;
        Code = string.Empty;
        Title = string.Empty;
        Text = string.Empty;
        Description = string.Empty;
        From = string.Empty;
    }

    public bool SaveToDataBase()
    {
        return DataSource.UpdateData(DataBaseName + "_Manager",
            new[]
            {
                    "ID",ID.ToString(),
                    "@Kodas",Code,
                    "@Antraste", Title,
                    "@Aprasymas", Description,
                    "@Turinys", Text,
                    "@Nuo",From
            });
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
    public void FillDataByCode(string Code)
    {
        DataView dw = DataSource.ExecuteSelectSQL($"SELECT * FROM {DataBaseName} WHERE Kodas = '{Code}'");

        if(dw != null && dw.Table.Rows.Count > 0)
        {
            this.ID = Convert.ToInt32(dw.Table.Rows[0]["ID"].ToString());
            this.Code = dw.Table.Rows[0]["Kodas"].ToString();
            this.Title = dw.Table.Rows[0]["Antraste"].ToString();
            this.Text = dw.Table.Rows[0]["Turinys"].ToString();
            this.Description = dw.Table.Rows[0]["Aprasymas"].ToString();
            this.From = dw.Table.Rows[0]["Nuo"].ToString();
        }
    }
}