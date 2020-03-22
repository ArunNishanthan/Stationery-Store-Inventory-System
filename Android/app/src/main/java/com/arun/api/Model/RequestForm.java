package com.arun.api.Model;

public class RequestForm {
     int Id;
     String Comments;
     int FormStatus;

    public int getHandledBy() {
        return HandledBy;
    }

    public void setHandledBy(int handledBy) {
        HandledBy = handledBy;
    }

    int HandledBy;

    public int getId() {
        return Id;
    }

    public void setId(int id) {
        Id = id;
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
}
