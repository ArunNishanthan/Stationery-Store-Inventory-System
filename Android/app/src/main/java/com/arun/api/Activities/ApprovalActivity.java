package com.arun.api.Activities;

import androidx.appcompat.app.AppCompatActivity;
import androidx.swiperefreshlayout.widget.SwipeRefreshLayout;

import android.content.Intent;
import android.graphics.Color;
import android.graphics.PorterDuff;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import com.arun.api.Adaptor.ApprovalAdaptor;
import com.arun.api.AsyncTask.Get.ApprovalAsyncTask;
import com.arun.api.AsyncTask.Get.RetrievalAsyncTask;
import com.arun.api.Model.Employee;
import com.arun.api.Model.RequisationForm;
import com.arun.api.Model.User;
import com.arun.api.R;
import com.google.android.material.floatingactionbutton.FloatingActionButton;
import com.google.android.material.snackbar.Snackbar;
import com.google.android.material.tabs.TabLayout;

import java.util.List;

/*
Coded by
Arun Nishanthan Anbalagan
 */
public class ApprovalActivity extends AppCompatActivity implements SwipeRefreshLayout.OnRefreshListener, ApprovalAdaptor.Icallback, ApprovalAsyncTask.IApprovalcallback {
    ListView listview;
    Employee employee;
    TextView tvMessage;
    SwipeRefreshLayout swipeRefreshLayout;
    User user;
    String id;

    FloatingActionButton fab;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_approval);

        Intent caller = getIntent();
        user = (User) caller.getSerializableExtra("User");

        if (user.getId() < 0) {
            Intent goBackIntent = new Intent(this, LoginActivity.class);
            startActivity(goBackIntent);
        }
        listview = findViewById(R.id.approvalList);
        tvMessage = findViewById(R.id.empty);

        swipeRefreshLayout = findViewById(R.id.swipeRefreshApproval);
        swipeRefreshLayout.setOnRefreshListener(this);

        tvMessage.setVisibility(View.INVISIBLE);

        fab = findViewById(R.id.fab);
        fab.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Toast.makeText(getApplicationContext(), "We are logging you out", Toast.LENGTH_SHORT).show();
                Intent logoutIntent = new Intent(getApplicationContext(), LoginActivity.class);
                startActivity(logoutIntent);

            }
        });
        employee = new Employee();
        employee.setId(user.getId());
        id = String.valueOf(user.getId());

        ApprovalAsyncTask approvalAsyncTask = new ApprovalAsyncTask(ApprovalActivity.this);
        approvalAsyncTask.execute(id);

        TabLayout tabLayout = findViewById(R.id.tabs);
        tabLayout.setTabTextColors(Color.WHITE, Color.RED);
        tabLayout.getTabAt(1).getIcon().setColorFilter(Color.WHITE, PorterDuff.Mode.SRC_IN);
        tabLayout.getTabAt(2).getIcon().setColorFilter(Color.WHITE, PorterDuff.Mode.SRC_IN);
        tabLayout.getTabAt(0).getIcon().setColorFilter(Color.RED, PorterDuff.Mode.SRC_IN);

        if (user.getRole() == 3) {
            tabLayout.removeTab(tabLayout.getTabAt(1));
            tabLayout.removeTab(tabLayout.getTabAt(1));
        }


        tabLayout.addOnTabSelectedListener(new TabLayout.OnTabSelectedListener() {
            @Override
            public void onTabSelected(TabLayout.Tab tab) {
                tab.getIcon().setColorFilter(Color.RED, PorterDuff.Mode.SRC_IN);

                if (tab.getPosition() == 1) {
                    Intent assignRepIntent = new Intent(getBaseContext(), AssignRepActivity.class);
                    assignRepIntent.putExtra("User", user);
                    startActivity(assignRepIntent);
                } else if (tab.getPosition() == 2) {
                    Intent delegationIntent = new Intent(getBaseContext(), DelegationActivity.class);
                    delegationIntent.putExtra("User", user);
                    startActivity(delegationIntent);
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
    public void getRequisationForms(List<RequisationForm> requisationForms) {
        ApprovalAdaptor adapter = new ApprovalAdaptor(this,
                R.layout.list_view_approval, requisationForms, employee);
        listview.setAdapter(adapter);
        if (listview.getAdapter().getCount() == 0) {
            tvMessage.setVisibility(View.VISIBLE);
        } else {
            tvMessage.setVisibility(View.INVISIBLE);
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
        ApprovalAsyncTask approvalAsyncTask = new ApprovalAsyncTask(this);
        approvalAsyncTask.execute(id);
    }
}
