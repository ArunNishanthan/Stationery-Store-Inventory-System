package com.arun.api.Model;

import java.io.Serializable;
import java.util.Collection;

public class DepDisbursementList implements Serializable {
    public int DepDisbursementListId;
    public String DepDisbursementListNumber;
    public Department Department;
    public String OTP;
    public String DisburseDate;
    public int DisbursementStatus;

    public Collection<DisburseItem> DisburseItems;
    public Boolean isGenerated;

    public Boolean getGenerated() {
        return isGenerated;
    }

    public void setGenerated(Boolean generated) {
        isGenerated = generated;
    }

    public int getDepDisbursementListId() {
        return DepDisbursementListId;
    }

    public void setDepDisbursementListId(int depDisbursementListId) {
        DepDisbursementListId = depDisbursementListId;
    }

    public String getDepDisbursementListNumber() {
        return DepDisbursementListNumber;
    }

    public void setDepDisbursementListNumber(String depDisbursementListNumber) {
        DepDisbursementListNumber = depDisbursementListNumber;
    }

    public com.arun.api.Model.Department getDepartment() {
        return Department;
    }

    public void setDepartment(com.arun.api.Model.Department department) {
        Department = department;
    }

    public String getOTP() {
        return OTP;
    }

    public void setOTP(String OTP) {
        this.OTP = OTP;
    }

    public String getDisburseDate() {
        return DisburseDate;
    }

    public void setDisburseDate(String disburseDate) {
        DisburseDate = disburseDate;
    }

    public int getDisbursementStatus() {
        return DisbursementStatus;
    }

    public void setDisbursementStatus(int disbursementStatus) {
        DisbursementStatus = disbursementStatus;
    }

    public Collection<DisburseItem> getDisburseItems() {
        return DisburseItems;
    }

    public void setDisburseItems(Collection<DisburseItem> disburseItems) {
        DisburseItems = disburseItems;
    }
}
