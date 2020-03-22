package com.arun.api.Model;

public class Department {

    String DepartmentCode;
    String DepartmentName;
    Employee DepartmentHead;
    Employee DepartmentRepresentative;
    Employee DepartmentActingHead;
    CollectionPoint CollectionPoint;

    public String getDepartmentCode() {
        return DepartmentCode;
    }

    public void setDepartmentCode(String departmentCode) {
        DepartmentCode = departmentCode;
    }

    public String getDepartmentName() {
        return DepartmentName;
    }

    public void setDepartmentName(String departmentName) {
        DepartmentName = departmentName;
    }

    public Employee getDepartmentHead() {
        return DepartmentHead;
    }

    public void setDepartmentHead(Employee departmentHead) {
        DepartmentHead = departmentHead;
    }

    public Employee getDepartmentRepresentative() {
        return DepartmentRepresentative;
    }

    public void setDepartmentRepresentative(Employee departmentRepresentative) {
        DepartmentRepresentative = departmentRepresentative;
    }

    public Employee getDepartmentActingHead() {
        return DepartmentActingHead;
    }

    public void setDepartmentActingHead(Employee departmentActingHead) {
        DepartmentActingHead = departmentActingHead;
    }

    public com.arun.api.Model.CollectionPoint getCollectionPoint() {
        return CollectionPoint;
    }

    public void setCollectionPoint(com.arun.api.Model.CollectionPoint collectionPoint) {
        CollectionPoint = collectionPoint;
    }
}
