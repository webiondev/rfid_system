using System;

//define class for reports on earning
public class EarningReport
{

    private string filter;//declare filter for....
    private string type;//declare type for...

    //default constructor
	public EarningReport()
	{
        
	}

    //value setting constructor
    public EarningReport(string filter, string type)
    {
        this.filter = filter;
        this.type = type;
    }

    //setter and getter

    public void filter 
    {
        get
        {

            return filter;
        }

        set
        {
            this.filter = "";

        }
    }

    //setter and getter

    public void type 
    {
        get
        {

            return type;
        }

        set
        {
            this.type = "";

        }
    }


}
