package com.arun.api.Model;

import java.io.Serializable;

public class User implements Serializable {
    int UserId;
    String UserName;
    String Email;
    String Password;

    public int getId() {
        return UserId;
    }

    public void setId(int id) {
        UserId = id;
    }

    public String getUserName() {
        return UserName;
    }

    public String getEmail() {
        return Email;
    }

    public String getPassword() {
        return Password;
    }

    public int getRole() {
        return Role;
    }

    public void setUserName(String userName) {
        UserName = userName;
    }

    public void setEmail(String email) {
        Email = email;
    }

    public void setPassword(String password) {
        Password = password;
    }

    public void setRole(int role) {
        Role = role;
    }

    int Role;
}
