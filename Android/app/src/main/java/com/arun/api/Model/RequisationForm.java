package com.arun.api.Model;

import com.google.gson.annotations.Expose;


import java.io.Serializable;
import java.util.List;

public class RequisationForm implements Serializable {
    @Expose
    int Id;
    @Expose
    String RequestNumber;
    @Expose
    Employee RequestedBy;
    @Expose
    Employee HandeledBy;
    @Expose
    String Comments;
    @Expose
    int FormStatus;
    @Expose
    String RequestedDate;
    @Expose
    String HandeledDate;

    public String getRequestedDate() {
        return RequestedDate;
    }

    public void setRequestedDate(String requestedDate) {
        RequestedDate = requestedDate;
    }

    public String getHandeledDate() {
        return HandeledDate;
    }

    public void setHandeledDate(String handeledDate) {
        HandeledDate = handeledDate;
    }

    @Expose
    List<RequestItem> RequestItems;

    public int getId() {
        return Id;
    }

    public void setId(int id) {
        Id = id;
    }

    public String getRequestNumber() {
        return RequestNumber;
    }

    public void setRequestNumber(String requestNumber) {
        RequestNumber = requestNumber;
    }

    public Employee getRequestedBy() {
        return RequestedBy;
    }

    public void setRequestedBy(Employee requestedBy) {
        RequestedBy = requestedBy;
    }

    public Employee getHandeledBy() {
        return HandeledBy;
    }

    public void setHandeledBy(Employee handeledBy) {
        HandeledBy = handeledBy;
    }

    public String getComments() {
        return Comments;
    }

    public void setComments(String comments) {
        Comments = comments;
    }

    public int getFormStatus() {
        return FormStatus;
    }

    public void setFormStatus(int formStatus) {
        FormStatus = formStatus;
    }

    public List<RequestItem> getRequestItems() {
        return RequestItems;
    }

    public void setRequestItems(List<RequestItem> requestItems) {
        RequestItems = requestItems;
    }
}
