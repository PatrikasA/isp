﻿using Faxai.Helper;

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
}