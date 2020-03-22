package com.arun.api.ViewModel;

import com.arun.api.Model.RetrievalList;

import java.util.List;

public class RetrivalViewModel {
     String RequisationFormsID;
     List<RetrievalList> RetrivalLists;

    public String getRequisationFormsID() {
        return RequisationFormsID;
    }

    public void setRequisationFormsID(String requisationFormsID) {
        RequisationFormsID = requisationFormsID;
    }

    public List<RetrievalList> getRetrivalLists() {
        return RetrivalLists;
    }

    public void setRetrivalLists(List<RetrievalList> retrivalLists) {
        RetrivalLists = retrivalLists;
    }
}
