package com.arun.api.Model;

import java.io.Serializable;

public class RequestItem implements Serializable {
    int RequestItemId;
    Item Item;
    int Quantity;

    public int getRequestItemId() {
        return RequestItemId;
    }

    public void setRequestItemId(int requestItemId) {
        RequestItemId = requestItemId;
    }

    public com.arun.api.Model.Item getItem() {
        return Item;
    }

    public void setItem(com.arun.api.Model.Item item) {
        Item = item;
    }

    public int getQuantity() {
        return Quantity;
    }

    public void setQuantity(int quantity) {
        Quantity = quantity;
    }


}
