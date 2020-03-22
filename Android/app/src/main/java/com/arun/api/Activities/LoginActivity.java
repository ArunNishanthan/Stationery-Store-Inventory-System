package com.arun.api.Activities;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;


import com.arun.api.AsyncTask.Get.LoginAsyncTask;
import com.arun.api.Model.User;
import com.arun.api.R;
/*
Coded by
Arun Nishanthan Anbalagan
 */
public class LoginActivity extends AppCompatActivity implements LoginAsyncTask.Icallback {

    EditText userName;
    EditText password;
    Button btn;
    private static final int TIME_INTERVAL = 2000;
    private long mBackPressed;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        userName = findViewById(R.id.et_email);
        password = findViewById(R.id.et_password);
        btn = findViewById(R.id.btn_login);

        btn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                LoginAsyncTask loginAsyncTask = new LoginAsyncTask(LoginActivity.this);
                loginAsyncTask.execute(userName.getText().toString(), password.getText().toString());
            }
        });

    }

    @Override
    public void checkUserLogin(User user) {
        Boolean isOther = false;
        if (user == null) {
            Toast.makeText(this, "Incorrect username or Password", Toast.LENGTH_SHORT).show();
        } else {
            Intent targetIntent = new Intent();
            if (user.getRole() == 1) {
                //Rep
                targetIntent= new Intent(this,RepresentativeActivity.class);
            } else if (user.getRole() == 2) {
                //Dep Head
                targetIntent = new Intent(this, ApprovalActivity.class);

            } else if (user.getRole() == 3) {
                //Act Dep Head
                targetIntent = new Intent(this, ApprovalActivity.class);
            } else if (user.getRole() > 3) {
                //Store
                targetIntent = new Intent(this, RequisaitionActivity.class);
            } else if (user.getRole() ==0 ) {
                isOther = true;
                Toast.makeText(this, "You are not Authorized, " + user.getUserName(), Toast.LENGTH_SHORT).show();
            }
            if (!isOther) {
                Toast.makeText(this, "Welcome back, " + user.getUserName(), Toast.LENGTH_SHORT).show();
                targetIntent.putExtra("User", user);
                startActivity(targetIntent);
            }
        }
    }

    @Override
    public void onBackPressed() {
        if (mBackPressed + TIME_INTERVAL > System.currentTimeMillis()) {
            finishAffinity();
            finish();
        } else {
            Toast.makeText(getBaseContext(), "Tap back button in order to exit", Toast.LENGTH_SHORT).show();
        }

        mBackPressed = System.currentTimeMillis();
    }
}
