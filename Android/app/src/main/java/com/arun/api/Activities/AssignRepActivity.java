package com.arun.api.Activities;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.graphics.Color;
import android.graphics.PorterDuff;
import android.os.Bundle;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;

import com.arun.api.AsyncTask.Get.AssignRepAsyncTask;
import com.arun.api.AsyncTask.Post.AssignRepPostAsyncTask;
import com.arun.api.Model.ChangeRep;
import com.arun.api.Model.CollectionPoint;
import com.arun.api.Model.Employee;
import com.arun.api.Model.User;
import com.arun.api.R;
import com.arun.api.ViewModel.AssignDeptRepViewModel;
import com.google.android.material.floatingactionbutton.FloatingActionButton;
import com.google.android.material.tabs.TabLayout;

import java.util.ArrayList;
/*
Coded by
Arun Nishanthan Anbalagan
 */
public class AssignRepActivity extends AppCompatActivity implements AssignRepAsyncTask.Icallback {

    Spinner spRep;
    Spinner spCP;
    User user;
    Button btnSubmit;
    TextView tvCollectionPoint, tvCurrentRep;
    FloatingActionButton fab;
    AssignDeptRepViewModel aassignDeptRepViewModel;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_assign_rep);

        Intent caller = getIntent();
        user = (User) caller.getSerializableExtra("User");

        if (user.getId() < 0) {
            Intent goBackIntent = new Intent(this, LoginActivity.class);
            startActivity(goBackIntent);
        }
        AssignRepAsyncTask assignRepAsyncTask = new AssignRepAsyncTask(this);
        assignRepAsyncTask.execute(String.valueOf(user.getId()));

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
        TabLayout.Tab tab = tabLayout.getTabAt(1);
        tab.select();
        tabLayout.getTabAt(0).getIcon().setColorFilter(Color.WHITE, PorterDuff.Mode.SRC_IN);
        tabLayout.getTabAt(1).getIcon().setColorFilter(Color.RED, PorterDuff.Mode.SRC_IN);
        tabLayout.getTabAt(2).getIcon().setColorFilter(Color.WHITE, PorterDuff.Mode.SRC_IN);
        tabLayout.setSelectedTabIndicatorColor(Color.RED);
        tabLayout.addOnTabSelectedListener(new TabLayout.OnTabSelectedListener() {
            @Override
            public void onTabSelected(TabLayout.Tab tab) {
                tab.getIcon().setColorFilter(Color.RED, PorterDuff.Mode.SRC_IN);

                if (tab.getPosition() == 0) {
                    Intent approvalActivity = new Intent(getBaseContext(), ApprovalActivity.class);
                    approvalActivity.putExtra("User", user);
                    startActivity(approvalActivity);
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

        spRep = findViewById(R.id.spinNewRep);
        spCP = findViewById(R.id.spinCP);

        btnSubmit = findViewById(R.id.btnSubmit);

        tvCollectionPoint = findViewById(R.id.currentCP);
        tvCurrentRep = findViewById(R.id.currrentRep);

        btnSubmit.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                ChangeRep changeRep = new ChangeRep();
                changeRep.setEmployeeName(spRep.getSelectedItem().toString());
                changeRep.setCollectionPointName(spCP.getSelectedItem().toString());

                AssignRepPostAsyncTask assignRepPostAsyncTask = new AssignRepPostAsyncTask();
                assignRepPostAsyncTask.execute(changeRep);

                finish();
                startActivity(getIntent());

            }
        });

    }

    @Override
    public void getAssignRepModel(AssignDeptRepViewModel assignDeptRepViewModel) {
        aassignDeptRepViewModel = assignDeptRepViewModel;

        ArrayList<String> emps = new ArrayList<>();
        ArrayList<String> cps = new ArrayList<>();

        for (Employee emp : assignDeptRepViewModel.getEmployees()) {
            emps.add(emp.getUserName());
        }

        for (CollectionPoint collectionPoint : assignDeptRepViewModel.getCollectionPoints()) {
            cps.add(collectionPoint.getPlace());
        }
        btnSubmit.setClickable(true);
        btnSubmit.setVisibility(View.VISIBLE);
        if (cps.size() <= 0 && emps.size() <= 0) {
            btnSubmit.setClickable(false);
            btnSubmit.setVisibility(View.INVISIBLE);
        }
        ArrayAdapter<String> spinRepadapter = new ArrayAdapter<String>(this,
                android.R.layout.simple_spinner_item, emps);

        spinRepadapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        spRep.setAdapter(spinRepadapter);

        ArrayAdapter<String> spinCPadapter = new ArrayAdapter<String>(this,
                android.R.layout.simple_spinner_item, cps);

        spinCPadapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        spCP.setAdapter(spinCPadapter);

        String rep = "Not Assigned";
        String cp = "Not Assigned";
        if (assignDeptRepViewModel.getDepartment().getDepartmentRepresentative() != null) {
            rep = assignDeptRepViewModel.getDepartment().getDepartmentRepresentative().getUserName();
        }
        if (assignDeptRepViewModel.getDepartment().getCollectionPoint() != null) {
            cp = assignDeptRepViewModel.getDepartment().getCollectionPoint().getPlace();
        }
        tvCurrentRep.setText(rep);
        tvCollectionPoint.setText(cp);
    }
}
