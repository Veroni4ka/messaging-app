//*****************************************************************************************
//*                                                                                       *
//* This is an auto-generated file by Microsoft ML.NET CLI (Command-Line Interface) tool. *
//*                                                                                       *
//*****************************************************************************************

using Microsoft.ML.Data;

namespace MessagingAppML.Model.DataModels
{
    public class ModelInput
    {
        [ColumnName("content"), LoadColumn(0)]
        public string Content { get; set; }


        [ColumnName("label"), LoadColumn(1)]
        public bool Label { get; set; }


    }
}
