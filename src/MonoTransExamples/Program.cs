using System;
using System.Data;
using System.Globalization;
using Npgsql;
using NpgsqlTypes;


namespace MonoTransExamples
{
    class Program
    {

    static readonly string connString = "Server=127.0.0.1;Port=5432;Database=myinvoices;User ID=martin;Password=Pa$$W0rd";

    static readonly string commandText1 = "INSERT INTO invoices(invoice_number,invoice_date,invoice_total)" +
        "VALUES(:number,:date,:total)";
    static readonly string commandText2 = "SELECT MAX(invoice_id) FROM invoices";
    static readonly string commandText3 = "INSERT INTO invoicedetails(invoice_id,invoiced_description,invoiced_quantity,invoiced_amount)" +
        "VALUES(:id,:description,:quantity,:amount)";
    static readonly string commandText4 = "UPDATE invoices SET invoice_total = :total WHERE invoice_id = :id";

static void Main(string[] args)
{
            Utilities.SetTitle("ADO.NET Transactions Sample");
    bool success = false;
    int recordsAffected = 0;
    Invoice invoice = new Invoice { 
        Invoice_number = 2099,
        Invoice_date = new DateTime(2013,01,29,6,6,6,100,Calendar.CurrentEra)
    };
    Invoicedetails[] details = { 
    new Invoicedetails{
        Invoice = invoice,
        Invoiced_description = "walkie-talkie 22-Channel",
        Invoiced_quantity = 3,
        Invoiced_amount = 19.99M
    },
    new Invoicedetails{
        Invoice = invoice,
        Invoiced_description = "2 GB SD Memory Card",
        Invoiced_quantity = 4,
        Invoiced_amount = 6.99M
    }
    };
    NpgsqlTransaction transaction = null;
    NpgsqlConnection conn = null;
    try
    {
    conn = new NpgsqlConnection(connString);
    conn.Open();
        transaction = conn.BeginTransaction();
        using (NpgsqlCommand cmd1 = new NpgsqlCommand(commandText1, conn, transaction))
        {
            cmd1.CommandType = CommandType.Text;
            cmd1.Parameters.Add("number", NpgsqlDbType.Integer, 4).Value = invoice.Invoice_number;
            cmd1.Parameters.Add("date", NpgsqlDbType.Timestamp).Value = invoice.Invoice_date;
            cmd1.Parameters.Add("total", NpgsqlDbType.Money).Value = invoice.Invoice_total;
            recordsAffected = cmd1.ExecuteNonQuery();
                    Utilities.PrintMessage(recordsAffected + " invoiced inserted");
        }

        if (recordsAffected > 0)
        {
            using (NpgsqlCommand cmd2 = new NpgsqlCommand(commandText2, conn, transaction))
            {
                cmd2.CommandType = CommandType.Text;
                invoice.Invoice_id = Convert.ToInt32(cmd2.ExecuteScalar());
                Utilities.PrintMessage("Invoice Id  " + invoice.Invoice_id);
            }
        }
        if (invoice.Invoice_id > 0)
        {
            recordsAffected = 0;
            foreach (Invoicedetails invd in details)
            {
                invd.Invoice.Invoice_id = invoice.Invoice_id;
                using (NpgsqlCommand cmd3 = new NpgsqlCommand(commandText3, conn, transaction))
                {
                    cmd3.CommandType = CommandType.Text;
                    cmd3.Parameters.Add("id", NpgsqlDbType.Integer, 4).Value = invd.Invoice.Invoice_id;
                    cmd3.Parameters.Add("description", NpgsqlDbType.Varchar, 512).Value = invd.Invoiced_description;
                    cmd3.Parameters.Add("quantity", NpgsqlDbType.Smallint).Value = invd.Invoiced_quantity;
                    cmd3.Parameters.Add("amount", NpgsqlDbType.Money).Value = invd.Invoiced_amount;
                    recordsAffected += cmd3.ExecuteNonQuery();
                }
                invoice.Invoice_total += invd.Invoiced_amount * invd.Invoiced_quantity;
            }
            Utilities.PrintMessage("Total: " + invoice.Invoice_total + " ,"+ recordsAffected + " records affected ");
            
        }
        if (recordsAffected == details.Length)
        {
            using (NpgsqlCommand cmd4 = new NpgsqlCommand(commandText4, conn, transaction)) 
            {
                cmd4.CommandType = CommandType.Text;
                cmd4.Parameters.Add("total",NpgsqlDbType.Money).Value = invoice.Invoice_total;
                cmd4.Parameters.Add("id",NpgsqlDbType.Integer).Value = invoice.Invoice_id;
                recordsAffected = cmd4.ExecuteNonQuery();
            }
                    Utilities.PrintMessage("Updated invoice " + invoice.Invoice_id + " with total "+ invoice.Invoice_total);
        }
        if(recordsAffected > 0)
                success = true;
    }
    catch(NpgsqlException ex)
    {
                Utilities.PrintMessage(ex.Message);
    }
    finally
    {
        if (success)
            transaction.Commit();
        else
            transaction.Rollback();
        if (conn != null)
            if (conn.State == ConnectionState.Open)
                conn.Close();
    }
            Utilities.PrintMessage("Done!");
            Utilities.Pause();
}
}

    class Invoice 
    {
    public int Invoice_id { set; get; }
    public int Invoice_number { set; get; }
    public DateTime Invoice_date { set; get; }
    public Decimal Invoice_total { set; get; }

    }

    class Invoicedetails 
    {
    public int Invoiced_id { set; get; }
    public Invoice Invoice { set; get; }
    public string Invoiced_description { set; get; }
    public int Invoiced_quantity { set; get; }
    public Decimal Invoiced_amount { set; get; }
    }
}
