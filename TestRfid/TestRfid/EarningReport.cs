using System;

//define class for reports on new asset acquirement
//this class generates final report on asset acquirement
public class EarningReport
{

  private string filter;//declare filter for....dd
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

    public string getFilter ()
    {
        
            return filter;
        
    }

    public void setFilter(string filter) {

        this.filter = filter;
    }

    //setter and getter

    public string getType()
    {

        return type;

    }

    public void setType(string type)
    {

        this.type = type;
    }



}
