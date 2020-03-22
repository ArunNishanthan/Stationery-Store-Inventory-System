package com.arun.api.Activities;

import android.content.Intent;
import android.graphics.Color;
import android.graphics.PorterDuff;
import android.os.Bundle;

import com.arun.api.Adaptor.DeptDisbursementAdapter;
import com.arun.api.AsyncTask.Get.DepDisbursementAsyncTask;
import com.arun.api.AsyncTask.Get.DeptDisbursementOtpCheckAsyncTask;
import com.arun.api.AsyncTask.Get.RequisationAsyncTask;
import com.arun.api.Model.DepDisbursementList;

import com.arun.api.R;
import com.google.android.material.floatingactionbutton.FloatingActionButton;
import com.google.android.material.snackbar.Snackbar;
import com.google.android.material.tabs.TabLayout;

import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;
import androidx.swiperefreshlayout.widget.SwipeRefreshLayout;

import android.view.View;
import android.widget.Button;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;


import java.util.ArrayList;
/*
Coded by
Arun Nishanthan Anbalagan
Aye Pwint Phyu
 */
public class DepDisbursementActivity extends AppCompatActivity implements DeptDisbursementOtpCheckAsyncTask.IDeptDiscallback, SwipeRefreshLayout.OnRefreshListener, DepDisbursementAsyncTask.ICallback, DeptDisbursementAdapter.IcallbackRestart {

    public ArrayList<DepDisbursementList> lists;
    public ListView listView;
    public Button btnCancel;
    TextView tvMessage;
    SwipeRefreshLayout swipeRefreshLayout;

    FloatingActionButton fab;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_dep_disbursement);


        tvMessage = findViewById(R.id.empty);
        tvMessage.setVisibility(View.INVISIBLE);

        listView = findViewById(R.id.listView3);
        btnCancel = findViewById(R.id.btnCancel);

        DepDisbursementAsyncTask depDisList = new DepDisbursementAsyncTask(this);
        depDisList.execute();

        swipeRefreshLayout = findViewById(R.id.swipeRefreshrequisation);
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

        TabLayout.Tab tab = tabLayout.getTabAt(2);
        tab.select();


        tabLayout.getTabAt(0).getIcon().setColorFilter(Color.WHITE, PorterDuff.Mode.SRC_IN);
        tabLayout.getTabAt(1).getIcon().setColorFilter(Color.WHITE, PorterDuff.Mode.SRC_IN);
        tabLayout.getTabAt(2).getIcon().setColorFilter(Color.RED, PorterDuff.Mode.SRC_IN);

        tabLayout.addOnTabSelectedListener(new TabLayout.OnTabSelectedListener() {
            @Override
            public void onTabSelected(TabLayout.Tab tab) {
                tab.getIcon().setColorFilter(Color.RED, PorterDuff.Mode.SRC_IN);

                if (tab.getPosition() == 0) {
                    Intent requisaitionActivityIndent = new Intent(getBaseContext(), RequisaitionActivity.class);
                    startActivity(requisaitionActivityIndent);
                }
                if (tab.getPosition() == 1) {
                    Intent retrievalIntent = new Intent(getBaseContext(), RetrievalActivity.class);
                    startActivity(retrievalIntent);
                }
            }

            @Override
            public void onTabUnselected(TabLayout.Tab tab) {
                tab.getIcon().setColorFilter(Color.WHITE, PorterDuff.Mode.SRC_IN);
            }

            @Override
            public void onTabReselected(TabLayout.Tab tab) {

            }
        });
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
            listView.setAdapter(adapter);
        }

        if (list.size() == 0) {
            tvMessage.setVisibility(View.VISIBLE);
            listView.setVisibility(View.INVISIBLE);
        } else {
            tvMessage.setVisibility(View.INVISIBLE);
            listView.setVisibility(View.VISIBLE);
        }

        swipeRefreshLayout.setRefreshing(false);
    }

    @Override
    public void onRestart() {
        onRefresh();
        super.onRestart();
    }

    @Override
    public void CheckOtp(DepDisbursementList depList) {
        DeptDisbursementOtpCheckAsyncTask deptDisListAsyncTask = new DeptDisbursementOtpCheckAsyncTask(this);
        deptDisListAsyncTask.execute(String.valueOf(depList.DepDisbursementListId), depList.OTP);

    }

    @Override
    public void onRefresh() {
        DepDisbursementAsyncTask depDisList = new DepDisbursementAsyncTask(this);
        depDisList.execute();
    }

    @Override
    public void getStatus(int status) {
        if (status != 1) {
            Toast.makeText(this, "Wrong OTP", Toast.LENGTH_SHORT).show();
        }else if(status == 1){
            Toast.makeText(this, "OTP is validated", Toast.LENGTH_SHORT).show();
        }
        onRefresh();
    }
}
