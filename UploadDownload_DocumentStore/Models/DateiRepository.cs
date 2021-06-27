using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;

namespace UploadDownload_DocumentStore.Models
{
    public class DateiRepository
    {
        private const string connectionString = @"Data Source=.\SQLEXPRESS;" +
                                            "Initial Catalog=NETDB;" +
                                            "Integrated Security=true;";



        public List<DateiBeschreibung> DateiListeAbrufen()
        {
            List<DateiBeschreibung> listeAllerDateien = new List<DateiBeschreibung>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT DocID, DocName, ContentType, InsertionDate FROM dbo.DocStore", connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        DateiBeschreibung eineDatei = new DateiBeschreibung
                        {
                            // ID
                            Id = reader.GetInt32(0),
                            // Name
                            Name = reader.GetString(1),
                            // ContentType
                            ContentType = reader.GetString(2),
                            // Datum
                            EinfügeDatum = reader.GetDateTime(3)
                        };

                        listeAllerDateien.Add(eineDatei);
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            return listeAllerDateien;
        }

        public string DateiInDatenbankSchreiben(IFormFile dieDatei)
        {
            string meldung = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(
                    "INSERT INTO dbo.DocStore (DocName, DocData, ContentType, ContentLength, InsertionDate)" +
                    "VALUES (@Name, @Data, @Type, @Length, @Date)"
                    , connection);

                    // Dateiname
                    command.Parameters.Add("@Name", SqlDbType.VarChar, 512).Value = Path.GetFileName(dieDatei.FileName);

                    // Dateiinhalt
                    command.Parameters.Add("@Data", SqlDbType.VarBinary).Value = dieDatei.OpenReadStream();

                    // Dateityp
                    command.Parameters.Add("@Type", SqlDbType.NVarChar, 100).Value = dieDatei.ContentType;

                    // Länge der Datei
                    command.Parameters.Add("@Length", SqlDbType.BigInt).Value = dieDatei.Length;

                    // Datum des Einfügens
                    command.Parameters.Add("@Date", SqlDbType.DateTime).Value = DateTime.Now;

                    command.ExecuteNonQuery();

                    // Meldung für ViewBag
                    meldung = "Datei \"" + Path.GetFileName(dieDatei.FileName) + "\" wurde hochgeladen";
                }
                catch (Exception e)
                {
                    meldung = e.Message;
                }
            }
            return meldung;
        }

        public Stream DateiAusDBLaden(int id)
        {
            Stream buffer = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand("SELECT DocData FROM dbo.DocStore WHERE DocID=@Id", connection);
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Dateiinhalt als Stream aus der DB lesen
                            buffer = reader.GetStream(0);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            return buffer;
        }
    }
}