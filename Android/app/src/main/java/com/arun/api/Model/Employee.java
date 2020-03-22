package com.arun.api.Model;

import java.io.Serializable;

public class Employee extends User implements Serializable {
    public String DepartmentCode;
    public Department Department;
}
