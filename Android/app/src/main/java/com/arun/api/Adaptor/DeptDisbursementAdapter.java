package com.arun.api.Adaptor;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.graphics.Color;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ListAdapter;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;


import com.arun.api.Activities.DepDisbursementActivity;
import com.arun.api.Activities.RepresentativeActivity;
import com.arun.api.AsyncTask.Get.DeptDisbursementCancelAsyncTask;
import com.arun.api.AsyncTask.Get.DeptDisbursementGenerateAsyncTask;
import com.arun.api.AsyncTask.Get.DeptDisbursementOtpCheckAsyncTask;
import com.arun.api.AsyncTask.Get.DeptDisbursementResendOtpCheckAsyncTask;
import com.arun.api.AsyncTask.Post.DeptDisbursementPostAsyncTask;
import com.arun.api.Model.DepDisbursementList;
import com.arun.api.Model.DisburseItem;
import com.arun.api.Model.DisburseRequest;
import com.arun.api.Model.RequestItem;
import com.arun.api.R;
import com.google.android.material.snackbar.Snackbar;

import java.util.ArrayList;
import java.util.List;

public class DeptDisbursementAdapter extends ArrayAdapter<DepDisbursementList> {

    private Context context;
    public ArrayList<DepDisbursementList> lists;
    StringBuilder ip_otp = new StringBuilder();
    private IcallbackRestart icallback;

    public DeptDisbursementAdapter(Context context, int resourceId, ArrayList<DepDisbursementList> lists) {
        super(context, resourceId, lists);
        this.context = context;
        this.lists = lists;
        this.icallback = (IcallbackRestart) context;
    }

    public View getView(final int position, View view, ViewGroup parent) {
        final DepDisbursementList rowItem = getItem(position);

        LayoutInflater inflater = (LayoutInflater) context.getSystemService(Activity.LAYOUT_INFLATER_SERVICE);
        view = inflater.inflate(R.layout.row_listview3, parent, false);

        Button btnAcknowledge, btnCancel, btnGenerate;
        String btnText = null;


        btnCancel = view.findViewById(R.id.btnCancel);

        btnCancel.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                DeptDisbursementCancelAsyncTask deptDisbursementCancelAsyncTask = new DeptDisbursementCancelAsyncTask();
                deptDisbursementCancelAsyncTask.execute(String.valueOf(rowItem.DepDisbursementListId));

                icallback.onRestart();
            }
        });


        btnAcknowledge = view.findViewById(R.id.btnAcknowledge);
        if (rowItem.getDisbursementStatus() == 3) {
            btnAcknowledge.setText("Acknowledge");

        } else if (rowItem.getDisbursementStatus() == 2) {

            btnAcknowledge.setText("Enter OTP");
        }
        btnGenerate = view.findViewById(R.id.btnGenerate);
        btnGenerate.setVisibility(View.GONE);
        btnGenerate.setClickable(false);

        if (btnAcknowledge.getText().toString().equalsIgnoreCase("Enter OTP")) {
            btnCancel.setClickable(false);
            btnCancel.setVisibility(View.GONE);
            for (DisburseItem disburseItem : rowItem.getDisburseItems()) {
                if (disburseItem.getDisbursedQuantity() < disburseItem.getRequestedQuantity() && !rowItem.isGenerated) {
                    btnGenerate.setVisibility(View.VISIBLE);
                    btnGenerate.setClickable(true);
                }
            }
        }
        btnGenerate.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                DeptDisbursementGenerateAsyncTask deptDisbursementGenerateAsyncTask = new DeptDisbursementGenerateAsyncTask();
                deptDisbursementGenerateAsyncTask.execute(String.valueOf(rowItem.DepDisbursementListId));
                Snackbar.make(view, "Generated", Snackbar.LENGTH_SHORT)
                        .show();
                icallback.onRestart();
            }
        });

        btnText = btnAcknowledge.getText().toString();

        if (btnText.equals("Acknowledge")) {

            btnAcknowledge.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    DisburseRequest disburseRequest = new DisburseRequest();

                    disburseRequest.Id=rowItem.getDepDisbursementListId();
                    disburseRequest.RequestItems= new ArrayList<>();
                    for (DisburseItem disItem:rowItem.getDisburseItems()) {
                        RequestItem requestItem = new RequestItem();
                        requestItem.setItem(disItem.RequestItem);
                        requestItem.setQuantity(disItem.DisbursedQuantity);
                        disburseRequest.RequestItems.add(requestItem);
                    }
                    Snackbar.make(v, "Acknowledged", Snackbar.LENGTH_SHORT)
                            .show();
                    DeptDisbursementPostAsyncTask changeDeptDisburse = new DeptDisbursementPostAsyncTask();
                    changeDeptDisburse.execute(disburseRequest);

                    icallback.onRestart();
                }
            });
        } else if (btnText.equals("Enter OTP")) {
            btnAcknowledge.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    alertDialog(rowItem);
                }
            });
        }

        if (rowItem != null && view != null) {
            TextView disNoTV = (TextView) view.findViewById(R.id.disbursement_textView);
            if (disNoTV != null) {
                disNoTV.setText(rowItem.getDepDisbursementListNumber());
            }

            TextView deptNameTV = (TextView) view.findViewById(R.id.deptName_textView);
            if (deptNameTV != null) {
                deptNameTV.setText(String.valueOf(rowItem.getDepartment().getDepartmentName()));
            }

            TextView deptRepTV = (TextView) view.findViewById(R.id.deptRep_textView);
            if (deptRepTV != null) {
                String rep = "Department Representative: " + String.valueOf(rowItem.getDepartment().getDepartmentRepresentative().getUserName());
                deptRepTV.setText(rep);
            }
            TextView collectionPointTV = (TextView) view.findViewById(R.id.collectionPoint_textView);
            if (collectionPointTV != null) {
                String cpname = "Collection Point: " + String.valueOf(rowItem.getDepartment().getCollectionPoint().getPlace());
                collectionPointTV.setText(cpname);
            }
            ListView lv = view.findViewById(R.id.listviewItem);
            if (lv != null) {
                ItemeAdapter itemAdapter = new ItemeAdapter(getContext(), R.layout.row_listview4, (List<DisburseItem>) rowItem.getDisburseItems(), rowItem.DisbursementStatus);
                lv.setAdapter(itemAdapter);
                setListViewHeightBasedOnChildren(lv);
            }

            if (this.context instanceof RepresentativeActivity) {
                btnAcknowledge.setVisibility(View.GONE);
                btnCancel.setVisibility(View.GONE);
                btnCancel.setVisibility(View.GONE);
                view.findViewById(R.id.btnLayout).setVisibility(View.GONE);
            }
        }
        return view;
    }

    private void setListViewHeightBasedOnChildren(ListView listView) {
        ListAdapter listAdapter = listView.getAdapter();
        if (listAdapter == null)
            return;

        int desiredWidth = View.MeasureSpec.makeMeasureSpec(listView.getWidth(), View.MeasureSpec.UNSPECIFIED);
        int totalHeight = 0;
        View view = null;
        for (int i = 0; i < listAdapter.getCount(); i++) {
            view = listAdapter.getView(i, view, listView);
            if (i == 0)
                view.setLayoutParams(new ViewGroup.LayoutParams(desiredWidth, ViewGroup.LayoutParams.WRAP_CONTENT));
            view.measure(desiredWidth, View.MeasureSpec.UNSPECIFIED);
            totalHeight += view.getMeasuredHeight();
        }
        ViewGroup.LayoutParams params = listView.getLayoutParams();
        params.height = totalHeight + (listView.getDividerHeight() * (listAdapter.getCount() - 1));
        listView.setLayoutParams(params);
        listView.requestLayout();

    }

    void alertDialog(DepDisbursementList depDisList) {
        final DepDisbursementList dl = depDisList;
        LayoutInflater li = LayoutInflater.from(getContext());
        View promptsView = li.inflate(R.layout.layout_otp, null);
        AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(getContext());
        alertDialogBuilder.setView(promptsView);
        final EditText userInput = promptsView.findViewById(R.id.otp_editText);

        alertDialogBuilder
                .setCancelable(false)
                .setPositiveButton("OK", new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int id) {
                        ip_otp = new StringBuilder();
                        ip_otp.append(userInput.getText());
                        dl.setOTP(ip_otp.toString());
                        if (!ip_otp.toString().contentEquals("")) {
                            postMethod(dl);
                        }
                    }
                })
                .setNegativeButton("Cancel", new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int id) {
                        dialog.cancel();
                    }
                }).setNeutralButton("Resend", new DialogInterface.OnClickListener() {
            public void onClick(DialogInterface dialogInterface, int id) {

                DeptDisbursementResendOtpCheckAsyncTask deptDisbursementResendOtpCheckAsyncTask = new DeptDisbursementResendOtpCheckAsyncTask();
                deptDisbursementResendOtpCheckAsyncTask.execute(String.valueOf(dl.DepDisbursementListId));
            }
        });
        AlertDialog alertDialog = alertDialogBuilder.create();
        alertDialog.show();

        Button b_pos_alert = alertDialog.getButton(DialogInterface.BUTTON_POSITIVE);
        if (b_pos_alert != null) {
            b_pos_alert.setTextColor(Color.BLACK);
        }
        Button b_pos_re = alertDialog.getButton(DialogInterface.BUTTON_NEGATIVE);
        if (b_pos_re != null) {
            b_pos_re.setTextColor(Color.BLACK);
        }
        Button b_pos_dr = alertDialog.getButton(DialogInterface.BUTTON_NEUTRAL);
        if (b_pos_dr != null) {
            b_pos_dr.setTextColor(Color.BLACK);
        }
    }

    public void postMethod(DepDisbursementList dl) {

        DepDisbursementList depList = new DepDisbursementList();
        depList.setOTP(ip_otp.toString());
        depList.setDisbursementStatus(dl.getDisbursementStatus());
        depList.setDisburseItems(dl.getDisburseItems());
        depList.setDisburseDate(dl.getDisburseDate());
        depList.setDepDisbursementListNumber(dl.getDepDisbursementListNumber());
        depList.setDepartment(dl.getDepartment());
        depList.setDepDisbursementListId(dl.getDepDisbursementListId());
        icallback.CheckOtp(depList);

    }

    public interface IcallbackRestart {
        void onRestart();

        void CheckOtp(DepDisbursementList depList);
    }
}
