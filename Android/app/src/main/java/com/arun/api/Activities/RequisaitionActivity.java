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

import com.arun.api.Adaptor.RequisationAdaptor;
import com.arun.api.AsyncTask.Get.RequisationAsyncTask;
import com.arun.api.Model.RequisitionByDepartmentItem;
import com.arun.api.R;
import com.google.android.material.floatingactionbutton.FloatingActionButton;
import com.google.android.material.tabs.TabLayout;

import java.util.ArrayList;
/*
Coded by
Arun Nishanthan Anbalagan
 */
public class RequisaitionActivity extends AppCompatActivity implements SwipeRefreshLayout.OnRefreshListener, RequisationAdaptor.Icallback, RequisationAsyncTask.IReqCallback {
    ListView listView;
    TextView tvMessage;
    SwipeRefreshLayout swipeRefreshLayout;

    FloatingActionButton fab;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_requisaition);

        tvMessage = findViewById(R.id.empty);

        tvMessage.setVisibility(View.INVISIBLE);

        listView = findViewById(R.id.reqList);
        RequisationAsyncTask requisationAsyncTask = new RequisationAsyncTask(this);
        requisationAsyncTask.execute();

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


        tabLayout.getTabAt(0).getIcon().setColorFilter(Color.RED, PorterDuff.Mode.SRC_IN);
        tabLayout.getTabAt(1).getIcon().setColorFilter(Color.WHITE, PorterDuff.Mode.SRC_IN);
        tabLayout.getTabAt(2).getIcon().setColorFilter(Color.WHITE, PorterDuff.Mode.SRC_IN);

        tabLayout.addOnTabSelectedListener(new TabLayout.OnTabSelectedListener() {
            @Override
            public void onTabSelected(TabLayout.Tab tab) {
                tab.getIcon().setColorFilter(Color.RED, PorterDuff.Mode.SRC_IN);

                if (tab.getPosition() == 1) {
                    Intent retrievalIntent = new Intent(getBaseContext(), RetrievalActivity.class);
                    startActivity(retrievalIntent);
                }
                if (tab.getPosition() == 2) {
                    Intent depRepIntent = new Intent(getBaseContext(), DepDisbursementActivity.class);
                    startActivity(depRepIntent);
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
    public void getRequisitionList(ArrayList<RequisitionByDepartmentItem> requisitionByDepartmentItems) {
        try {
            RequisationAdaptor adapter = new RequisationAdaptor(this,
                    R.layout.list_view_requisation, requisitionByDepartmentItems);
            listView.setAdapter(adapter);

            if (listView.getAdapter().getCount() == 0) {
                tvMessage.setVisibility(View.VISIBLE);
            } else {
                tvMessage.setVisibility(View.INVISIBLE);
            }
        } catch (Exception e) {
            System.out.println(e.getMessage());
        }
        swipeRefreshLayout.setRefreshing(false);

    }

    @Override
    public void onRestart() {
        super.onRestart();
        onRefresh();
    }

    @Override
    public void onRefresh() {
        RequisationAsyncTask requisationAsyncTask = new RequisationAsyncTask(this);
        requisationAsyncTask.execute();
    }
}
