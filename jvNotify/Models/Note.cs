using SQLite;
using System;

namespace jvNotify.Models
{
  public class Note
  {
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public string Text { get; set; }
    public DateTime StartDate { get; set; }
    public int OnGo { get; set; }
    public int Warn { get; set; }
    public int TypeDate { get; set; }
    public int Min { get; set; }
    public int Hrs { get; set; }
  }
}
