package com.arun.api.ViewModel;

import com.arun.api.Model.CollectionPoint;
import com.arun.api.Model.Department;
import com.arun.api.Model.Employee;

import java.util.ArrayList;

public class AssignDeptRepViewModel {
    ArrayList<CollectionPoint> CollectionPoints;
    ArrayList<Employee> Employees;
    Department Department;
    Employee AssignTo;
    CollectionPoint CollectionPoint;

    public com.arun.api.Model.Department getDepartment() {
        return Department;
    }

    public void setDepartment(com.arun.api.Model.Department department) {
        Department = department;
    }


    public ArrayList<com.arun.api.Model.CollectionPoint> getCollectionPoints() {
        return CollectionPoints;
    }

    public void setCollectionPoints(ArrayList<com.arun.api.Model.CollectionPoint> collectionPoints) {
        CollectionPoints = collectionPoints;
    }

    public ArrayList<Employee> getEmployees() {
        return Employees;
    }

    public void setEmployees(ArrayList<Employee> employees) {
        Employees = employees;
    }

    public Employee getAssignTo() {
        return AssignTo;
    }

    public void setAssignTo(Employee assignTo) {
        AssignTo = assignTo;
    }

    public com.arun.api.Model.CollectionPoint getCollectionPoint() {
        return CollectionPoint;
    }

    public void setCollectionPoint(com.arun.api.Model.CollectionPoint collectionPoint) {
        CollectionPoint = collectionPoint;
    }
}
