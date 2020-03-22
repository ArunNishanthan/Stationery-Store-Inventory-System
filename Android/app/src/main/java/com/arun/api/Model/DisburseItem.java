package com.arun.api.Model;

import java.io.Serializable;

public class DisburseItem implements Serializable {

    public int DisburseItemID;
    public Item RequestItem;
    public int RequestedQuantity;
    public int RetrivedQuantity;
    public int DisbursedQuantity;
    public  DepDisbursementList depDisbursementList;

    public int getDisburseItemID() {
        return DisburseItemID;
    }

    public void setDisburseItemID(int disburseItemID) {
        DisburseItemID = disburseItemID;
    }

    public Item getRequestItem() {
        return RequestItem;
    }

    public void setRequestItem(Item requestItem) {
        RequestItem = requestItem;
    }

    public int getRequestedQuantity() {
        return RequestedQuantity;
    }

    public void setRequestedQuantity(int requestedQuantity) {
        RequestedQuantity = requestedQuantity;
    }

    public int getRetrivedQuantity() {
        return RetrivedQuantity;
    }

    public void setRetrivedQuantity(int retrivedQuantity) {
        RetrivedQuantity = retrivedQuantity;
    }

    public int getDisbursedQuantity() {
        return DisbursedQuantity;
    }

    public void setDisbursedQuantity(int disbursedQuantity) {
        DisbursedQuantity = disbursedQuantity;
    }
}
