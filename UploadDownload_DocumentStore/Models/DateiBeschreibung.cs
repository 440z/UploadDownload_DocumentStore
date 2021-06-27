using System;

namespace UploadDownload_DocumentStore.Models
{
    public class DateiBeschreibung
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public long Länge { get; set; }
        public DateTime EinfügeDatum { get; set; }
    }
}