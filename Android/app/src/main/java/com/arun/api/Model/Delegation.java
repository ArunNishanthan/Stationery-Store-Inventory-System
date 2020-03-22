package com.arun.api.Model;

public class Delegation {
    int Id;
    Employee DelegatedTo;
    String FromDate;
    String ToDate;

    public int getId() {
        return Id;
    }

    public void setId(int id) {
        Id = id;
    }

    public Employee getDelegatedTo() {
        return DelegatedTo;
    }

    public void setDelegatedTo(Employee delegatedTo) {
        DelegatedTo = delegatedTo;
    }

    public String getFromDate() {
        return FromDate;
    }

    public void setFromDate(String fromDate) {
        FromDate = fromDate;
    }

    public String getToDate() {
        return ToDate;
    }

    public void setToDate(String toDate) {
        ToDate = toDate;
    }
}
