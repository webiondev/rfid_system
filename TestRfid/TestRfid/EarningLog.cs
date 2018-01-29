﻿using System;

//define class for log on new acquirement
//This class temporarily stores new acquirement for db operation
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
