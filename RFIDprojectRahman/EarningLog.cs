using System;

//define class for log on earning
public class EarningLog
{

    private string filter;//declare filter for....
    private string type;//declare type for...

    //default constructor
    public EarningLog()
    {

    }

    //value setting constructor
    public EarningLog(string filter, string type)
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
