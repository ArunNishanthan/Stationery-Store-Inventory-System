package com.arun.api.Activities;

import android.graphics.Color;
import android.graphics.PorterDuff;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.ListView;

import androidx.appcompat.app.AppCompatActivity;
import androidx.swiperefreshlayout.widget.SwipeRefreshLayout;

import java.util.ArrayList;

import android.content.Intent;
import android.widget.TextView;
import android.widget.Toast;

import com.arun.api.Adaptor.RetrievalAdapter;
import com.arun.api.AsyncTask.Get.RetrievalAsyncTask;
import com.arun.api.AsyncTask.Post.RetrievalPostAsyncTask;
import com.arun.api.Model.RetrievalList;
import com.arun.api.ViewModel.RetrivalViewModel;
import com.arun.api.R;
import com.google.android.material.floatingactionbutton.FloatingActionButton;
import com.google.android.material.tabs.TabLayout;
/*
Coded by
Arun Nishanthan Anbalagan
May Thandar Theint Aung
 */
public class RetrievalActivity extends AppCompatActivity implements SwipeRefreshLayout.OnRefreshListener, RetrievalAsyncTask.ICallback {
    public ArrayList<RetrievalList> lists;
    public ListView listView;
    public String formID;

    SwipeRefreshLayout swipeRefreshLayout;
    Button btn;
    TextView tvMessage;
    FloatingActionButton fab;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_retrieval);

        RetrievalAsyncTask retrievalAsyncTaskAsyncTask = new RetrievalAsyncTask(RetrievalActivity.this);
        retrievalAsyncTaskAsyncTask.execute();

        swipeRefreshLayout = findViewById(R.id.swipeRefreshRetrival);
        swipeRefreshLayout.setOnRefreshListener(this);

        btn = findViewById(R.id.btnSubmit);
        listView = findViewById(R.id.listview);

        tvMessage = findViewById(R.id.empty);

        tvMessage.setVisibility(View.INVISIBLE);
        btn.setVisibility(View.INVISIBLE);
        btn.setClickable(false);


        btn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                RetrivalViewModel retrivalViewModel = new RetrivalViewModel();
                retrivalViewModel.setRequisationFormsID(formID);
                retrivalViewModel.setRetrivalLists(lists);

                RetrievalPostAsyncTask retrievalPostAsyncTask = new RetrievalPostAsyncTask();
                retrievalPostAsyncTask.execute(retrivalViewModel);

               onRefresh();
            }
        });

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

        tabLayout.addOnTabSelectedListener(new TabLayout.OnTabSelectedListener() {
            @Override
            public void onTabSelected(TabLayout.Tab tab) {
                tab.getIcon().setColorFilter(Color.RED, PorterDuff.Mode.SRC_IN);

                if (tab.getPosition() == 0) {
                    Intent requisaitionActivityIndent = new Intent(getBaseContext(), RequisaitionActivity.class);
                    startActivity(requisaitionActivityIndent);
                } else if (tab.getPosition() == 2) {
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
    public void SentRetrievalList(ArrayList<RetrievalList> list, String formID) {
        lists = list;
        this.formID = formID;
        RetrievalAdapter adapter = new RetrievalAdapter(this, R.layout.row_listview, lists);
        listView.setAdapter(adapter);

        tvMessage.setVisibility(View.VISIBLE);

        btn.setVisibility(View.INVISIBLE);
        btn.setClickable(false);

        if (list.size() != 0) {
            tvMessage.setVisibility(View.INVISIBLE);
            btn.setVisibility(View.VISIBLE);
            btn.setClickable(true);
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
        RetrievalAsyncTask retrievalAsyncTaskAsyncTask = new RetrievalAsyncTask(RetrievalActivity.this);
        retrievalAsyncTaskAsyncTask.execute();
    }

}
