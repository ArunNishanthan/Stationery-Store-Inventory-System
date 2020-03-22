package com.arun.api.Model;

import com.google.gson.annotations.SerializedName;

import java.io.Serializable;
import java.util.List;

public class RetrievalList implements Serializable
{
    @SerializedName("RetrievedItem")
     String RetrievedItem;
    @SerializedName("Needed")
     int Needed ;
    @SerializedName("Retrieved")
     int Retrieved ;
    @SerializedName("Retrievals")
     List<Retrieval> Retrievals ;

    public String getRetrievedItem() {
        return RetrievedItem;
    }

    public void setRetrievedItem(String retrievedItem) {
        RetrievedItem = retrievedItem;
    }

    public int getNeeded() {
        return Needed;
    }

    public void setNeeded(int needed) {
        Needed = needed;
    }

    public int getRetrieved() {
        return Retrieved;
    }

    public void setRetrieved(int retrieved) {
        Retrieved = retrieved;
    }

    public List<Retrieval> getRetrievals() {
        return Retrievals;
    }

    public void setRetrievals(List<Retrieval> retrievals) {
        Retrievals = retrievals;
    }
}
