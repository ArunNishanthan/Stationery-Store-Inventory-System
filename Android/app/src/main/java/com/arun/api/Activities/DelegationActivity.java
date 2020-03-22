package com.arun.api.Activities;

import androidx.appcompat.app.AppCompatActivity;

import android.app.DatePickerDialog;
import android.content.Intent;
import android.graphics.Color;
import android.graphics.PorterDuff;
import android.icu.text.SimpleDateFormat;
import android.os.Bundle;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.DatePicker;
import android.widget.EditText;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;

import com.arun.api.AsyncTask.Get.DelegationCheckAsyncTask;
import com.arun.api.AsyncTask.Post.DelegatePostAsyncTask;
import com.arun.api.AsyncTask.Get.DelegationAsyncTask;
import com.arun.api.AsyncTask.Get.DelegationHistoryAsyncTask;
import com.arun.api.Model.Delegation;
import com.arun.api.Model.DelegationDTO;
import com.arun.api.Model.Employee;
import com.arun.api.Model.User;
import com.arun.api.R;
import com.arun.api.ViewModel.DelegateAuthorityViewModel;
import com.arun.api.ViewModel.DelegationHistoryViewModel;
import com.google.android.material.floatingactionbutton.FloatingActionButton;
import com.google.android.material.tabs.TabLayout;

import java.text.ParseException;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
/*
Coded by
Arun Nishanthan Anbalagan
 */
public class DelegationActivity extends AppCompatActivity implements DelegationCheckAsyncTask.IDelegationcallback, DelegationHistoryAsyncTask.IHistorycallback, DelegationAsyncTask.Icallback {

    EditText etFromDate, etToDate;
    User user;
    Button btnSubmit;
    Spinner spEmployee;
    DatePickerDialog picker;
    TextView tvHistory;
    String pattern = "dd/MM/yyyy";
    SimpleDateFormat simpleDateFormat = new SimpleDateFormat(pattern);
    FloatingActionButton fab;

    DelegationDTO delegationDTO;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_delegation);

        etFromDate = findViewById(R.id.etFromDate);
        etToDate = findViewById(R.id.etToDate);
        btnSubmit = findViewById(R.id.btnSubmit);
        spEmployee = findViewById(R.id.spinEmployee);
        tvHistory = findViewById(R.id.history);

        Intent caller = getIntent();
        user = (User) caller.getSerializableExtra("User");

        if (user.getId() < 0) {
            Intent goBackIntent = new Intent(this, LoginActivity.class);
            startActivity(goBackIntent);
        }

        fab = findViewById(R.id.fab);
        fab.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Toast.makeText(getApplicationContext(), "We are logging you out", Toast.LENGTH_SHORT).show();
                Intent logoutIntent = new Intent(getApplicationContext(), LoginActivity.class);
                startActivity(logoutIntent);

            }
        });

        final DelegationAsyncTask delegationAsyncTask = new DelegationAsyncTask(this);
        delegationAsyncTask.execute(String.valueOf(user.getId()));

        DelegationHistoryAsyncTask delegationHistoryAsyncTask = new DelegationHistoryAsyncTask(this);
        delegationHistoryAsyncTask.execute(String.valueOf(user.getId()));

        final Calendar cldr = Calendar.getInstance();


        etFromDate.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                int day = cldr.get(Calendar.DAY_OF_MONTH);
                int month = cldr.get(Calendar.MONTH);
                int year = cldr.get(Calendar.YEAR);
                picker = new DatePickerDialog(DelegationActivity.this, R.style.DialogTheme,
                        new DatePickerDialog.OnDateSetListener() {
                            @Override
                            public void onDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth) {
                                String date = dayOfMonth + "/" + (monthOfYear + 1) + "/" + year;
                                setDisplay(etFromDate, date);
                            }
                        }, year, month, day);

                picker.getDatePicker().setMinDate(new Date().getTime());
                picker.show();
            }
        });
        etToDate.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                int day = cldr.get(Calendar.DAY_OF_MONTH);
                int month = cldr.get(Calendar.MONTH);
                int year = cldr.get(Calendar.YEAR);
                picker = new DatePickerDialog(DelegationActivity.this, R.style.DialogTheme,
                        new DatePickerDialog.OnDateSetListener() {
                            @Override
                            public void onDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth) {
                                String date = dayOfMonth + "/" + (monthOfYear + 1) + "/" + year;
                                setDisplay(etToDate, date);
                            }
                        }, year, month, day);
                picker.getDatePicker().setMinDate(new Date().getTime());
                picker.show();
            }
        });
        btnSubmit.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                try {
                    Date fromDate = simpleDateFormat.parse(etFromDate.getText().toString());
                    Date toDate = simpleDateFormat.parse(etToDate.getText().toString());
                    if (fromDate.before(toDate) || fromDate.equals(toDate)) {
                         delegationDTO = new DelegationDTO();
                        delegationDTO.setFromDate(etFromDate.getText().toString());
                        delegationDTO.setToDate(etToDate.getText().toString());
                        Employee employee = new Employee();
                        employee.setUserName(spEmployee.getSelectedItem().toString());
                        delegationDTO.setDelegatedTo(employee);

                        DelegationCheckAsyncTask delegationCheckAsyncTask = new DelegationCheckAsyncTask(DelegationActivity.this);
                        delegationCheckAsyncTask.execute(delegationDTO);
                       
                    } else {
                        Toast.makeText(DelegationActivity.this, "Incorrect Dates. Please Check Again", Toast.LENGTH_SHORT).show();
                    }
                } catch (Exception e) {
                    System.out.println(e.getMessage());
                }
            }
        });

        TabLayout tabLayout = findViewById(R.id.tabs);
        tabLayout.setTabTextColors(Color.WHITE, Color.RED);

        TabLayout.Tab tab = tabLayout.getTabAt(2);
        tab.select();

        tabLayout.getTabAt(0).getIcon().setColorFilter(Color.WHITE, PorterDuff.Mode.SRC_IN);
        tabLayout.getTabAt(1).getIcon().setColorFilter(Color.WHITE, PorterDuff.Mode.SRC_IN);
        tabLayout.getTabAt(2).getIcon().setColorFilter(Color.RED, PorterDuff.Mode.SRC_IN);
        tabLayout.setSelectedTabIndicatorColor(Color.RED);

        tabLayout.addOnTabSelectedListener(new TabLayout.OnTabSelectedListener() {
            @Override
            public void onTabSelected(TabLayout.Tab tab) {
                tab.getIcon().setColorFilter(Color.RED, PorterDuff.Mode.SRC_IN);

                if (tab.getPosition() == 1) {
                    Intent assignRepIntent = new Intent(getBaseContext(), AssignRepActivity.class);
                    assignRepIntent.putExtra("User",user);
                    startActivity(assignRepIntent);
                } else if (tab.getPosition() == 0) {
                    Intent approvalActivity = new Intent(getBaseContext(), ApprovalActivity.class);
                    approvalActivity.putExtra("User",user);
                    startActivity(approvalActivity);
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
    public void getDelegateModel(DelegateAuthorityViewModel delegateAuthorityViewModel) {
        ArrayList<String> emps = new ArrayList<>();

        for (Employee emp : delegateAuthorityViewModel.getEmployees()) {
            emps.add(emp.getUserName());
        }
        ArrayAdapter<String> spinEmpAdapter = new ArrayAdapter<String>(this,
                android.R.layout.simple_spinner_item, emps);

        spinEmpAdapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        spEmployee.setAdapter(spinEmpAdapter);
    }


    void setDisplay(EditText editText, String date) {
        editText.setText(date);
    }

    @Override
    public void getHistory(DelegationHistoryViewModel delegationHistoryViewModel) {
        StringBuilder history = new StringBuilder();

        String pattern = "yyyy-MM-dd";
        String patternOut = "dd-MM-yyyy";
        SimpleDateFormat simpleDateFormat = new SimpleDateFormat(pattern);

        SimpleDateFormat simpleDateFormatOut = new SimpleDateFormat(patternOut);
int count =0;
        for (Delegation delegation : delegationHistoryViewModel.Delegations) {
            try {
                Date fromDate = simpleDateFormat.parse(delegation.getFromDate().substring(0, 10));
                Date toDate = simpleDateFormat.parse(delegation.getToDate().substring(0, 10));
                Date today = simpleDateFormat.parse(simpleDateFormat.format(new Date()));
                if (fromDate.after(today) || fromDate.equals(today) || toDate.after(today) || toDate.equals(today)) {
                    history.append(delegation.getDelegatedTo().getUserName())
                            .append("   ")
                            .append(simpleDateFormatOut.format(fromDate))
                            .append("   ")
                            .append(simpleDateFormatOut.format(toDate))
                            .append("\n");
                    count++;
                }
            } catch (ParseException e) {
                e.printStackTrace();
            }
        }
        if (count != 0)
            tvHistory.setText(history);
        else
            tvHistory.setText("No upcoming Delegations");
    }

    @Override
    public void getStatus(int status) {
        if (status==1) {
            DelegatePostAsyncTask delegatePostAsyncTask = new DelegatePostAsyncTask();
            delegatePostAsyncTask.execute(delegationDTO);
            finish();
            startActivity(getIntent());
        }
        else{
            Toast.makeText(this, "Employee already assigned", Toast.LENGTH_SHORT).show();
        }
    }
}
