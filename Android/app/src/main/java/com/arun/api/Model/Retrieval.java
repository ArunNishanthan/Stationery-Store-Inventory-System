package com.arun.api.Model;

import com.google.gson.annotations.SerializedName;

import java.io.Serializable;

public class Retrieval implements Serializable {
    @SerializedName("DepartmentCode")
    public String DepartmentCode;
    @SerializedName("Needed")
    public int Needed;
    @SerializedName("Actual")
    public int Actual;

    public String getDepartmentCode() {
        return DepartmentCode;
    }

    public void setDepartmentCode(String departmentCode) {
        DepartmentCode = departmentCode;
    }

    public int getNeeded() {
        return Needed;
    }

    public void setNeeded(int needed) {
        Needed = needed;
    }

    public int getActual() {
        return Actual;
    }

    public void setActual(int actual) {
        Actual = actual;
    }
}
