package com.arun.api.Activities;

import androidx.appcompat.app.AppCompatActivity;
import androidx.swiperefreshlayout.widget.SwipeRefreshLayout;

import android.content.Intent;
import android.graphics.Color;
import android.graphics.PorterDuff;
import android.os.Bundle;
import android.view.View;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import com.arun.api.Adaptor.DeptDisbursementAdapter;
import com.arun.api.AsyncTask.Get.DepDisbursementAsyncTask;
import com.arun.api.AsyncTask.Get.DepDisbursementbyIdAsyncTask;
import com.arun.api.Model.DepDisbursementList;
import com.arun.api.Model.Employee;
import com.arun.api.Model.User;
import com.arun.api.R;
import com.google.android.material.floatingactionbutton.FloatingActionButton;
import com.google.android.material.tabs.TabLayout;

import java.util.ArrayList;
/*
Coded by
Arun Nishanthan Anbalagan
 */
public class RepresentativeActivity extends AppCompatActivity implements DepDisbursementbyIdAsyncTask.ICallback, SwipeRefreshLayout.OnRefreshListener, DepDisbursementAsyncTask.ICallback, DeptDisbursementAdapter.IcallbackRestart {

    User user;
    Employee employee;
    TextView tvMessage;
    SwipeRefreshLayout swipeRefreshLayout;
    FloatingActionButton fab;
    String id;
    ListView lv;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_representative);

        Intent caller = getIntent();
        user = (User) caller.getSerializableExtra("User");

        if (user.getId() < 0) {
            Intent goBackIntent = new Intent(this, LoginActivity.class);
            startActivity(goBackIntent);
        }

        tvMessage = findViewById(R.id.empty);

        swipeRefreshLayout = findViewById(R.id.swipeRefreshApproval);
        swipeRefreshLayout.setOnRefreshListener(this);

        fab = findViewById(R.id.fab);
        fab.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Toast.makeText(getApplicationContext(), "We are logging you out", Toast.LENGTH_SHORT).show();
                Intent logoutIntent = new Intent(getApplicationContext(), LoginActivity.class);
                startActivity(logoutIntent);

            }
        });


        TabLayout tabLayout = findViewById(R.id.tabs);
        tabLayout.setTabTextColors(Color.WHITE, Color.RED);
        tabLayout.getTabAt(0).getIcon().setColorFilter(Color.WHITE, PorterDuff.Mode.SRC_IN);

        lv = findViewById(R.id.disburesementList);

        DepDisbursementbyIdAsyncTask depDisbursementbyIdAsyncTask = new DepDisbursementbyIdAsyncTask(this);
        depDisbursementbyIdAsyncTask.execute(String.valueOf(user.getId()));
    }

    @Override
    public void onRefresh() {
        DepDisbursementbyIdAsyncTask depDisbursementbyIdAsyncTask = new DepDisbursementbyIdAsyncTask(this);
        depDisbursementbyIdAsyncTask.execute(String.valueOf(user.getId()));
    }

    @Override
    public void onRestart() {
        super.onRestart();
        onRefresh();
    }

    @Override
    public void CheckOtp(DepDisbursementList depList) {

    }

    @Override
    public void sendDepDisbursementList(ArrayList<DepDisbursementList> list) {
        ArrayList<DepDisbursementList> lists = new ArrayList<>();
        for (DepDisbursementList dp : list) {
            if (dp.getDisburseItems().size() > 0) {
                lists.add(dp);
            }
        }
        if (lists.size() > 0) {
            DeptDisbursementAdapter adapter = new DeptDisbursementAdapter(this, R.layout.row_listview3, lists);
            lv.setAdapter(adapter);
        }

        if (list.size() == 0) {
            tvMessage.setVisibility(View.VISIBLE);
        } else {
            tvMessage.setVisibility(View.INVISIBLE);
        }

        swipeRefreshLayout.setRefreshing(false);
    }
}
