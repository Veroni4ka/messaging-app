// This file was auto-generated by ML.NET Model Builder. 

using Microsoft.ML.Data;

namespace MessagingAppML.Model.DataModels
{
    public class ModelInput
    {
        [ColumnName("id"), LoadColumn(0)]
        public string Id { get; set; }


        [ColumnName("comment_text"), LoadColumn(1)]
        public string Comment_text { get; set; }


        [ColumnName("toxic"), LoadColumn(2)]
        public string Toxic { get; set; }


        [ColumnName("severe_toxic"), LoadColumn(3)]
        public string Severe_toxic { get; set; }


        [ColumnName("obscene"), LoadColumn(4)]
        public string Obscene { get; set; }


        [ColumnName("threat"), LoadColumn(5)]
        public string Threat { get; set; }


        [ColumnName("insult"), LoadColumn(6)]
        public string Insult { get; set; }


        [ColumnName("identity_hate"), LoadColumn(7)]
        public string Identity_hate { get; set; }


    }
}
