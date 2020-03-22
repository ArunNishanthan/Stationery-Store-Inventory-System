package com.arun.api.ViewModel;

import com.arun.api.Model.DelegationDTO;
import com.arun.api.Model.Employee;

import java.util.ArrayList;

public class DelegateAuthorityViewModel {
    com.arun.api.Model.DelegationDTO DelegationDTO;
    ArrayList<Employee> Employees;

    public com.arun.api.Model.DelegationDTO getDelegationDTO() {
        return DelegationDTO;
    }

    public void setDelegationDTO(com.arun.api.Model.DelegationDTO delegationDTO) {
        DelegationDTO = delegationDTO;
    }

    public ArrayList<Employee> getEmployees() {
        return Employees;
    }

    public void setEmployees(ArrayList<Employee> employees) {
        Employees = employees;
    }
}
