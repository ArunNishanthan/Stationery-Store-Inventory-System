package com.arun.api.Model;

import java.io.Serializable;

public class Item implements Serializable {
    String ItemCode;
    String Category;
    String Description;
    int ReorderLevel;
    int ReorderQuantity;
    int CurrentQuantity;
    String UnitOfMeasure;
    Boolean IsActive;

    public String getItemCode() {
        return ItemCode;
    }

    public void setItemCode(String itemCode) {
        ItemCode = itemCode;
    }

    public String getCategory() {
        return Category;
    }

    public void setCategory(String category) {
        Category = category;
    }

    public String getDescription() {
        return Description;
    }

    public void setDescription(String description) {
        Description = description;
    }

    public int getReorderLevel() {
        return ReorderLevel;
    }

    public void setReorderLevel(int reorderLevel) {
        ReorderLevel = reorderLevel;
    }

    public int getReorderQuantity() {
        return ReorderQuantity;
    }

    public void setReorderQuantity(int reorderQuantity) {
        ReorderQuantity = reorderQuantity;
    }

    public int getCurrentQuantity() {
        return CurrentQuantity;
    }

    public void setCurrentQuantity(int currentQuantity) {
        CurrentQuantity = currentQuantity;
    }

    public String getUnitOfMeasure() {
        return UnitOfMeasure;
    }

    public void setUnitOfMeasure(String unitOfMeasure) {
        UnitOfMeasure = unitOfMeasure;
    }

    public Boolean getActive() {
        return IsActive;
    }

    public void setActive(Boolean active) {
        IsActive = active;
    }
}
