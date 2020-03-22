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
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;

import com.arun.api.AsyncTask.Post.ApprovalPostAsyncTask;
import com.arun.api.Model.Employee;
import com.arun.api.Model.RequestForm;
import com.arun.api.Model.RequestItem;
import com.arun.api.Model.RequisationForm;
import com.arun.api.R;


import java.util.List;

public class ApprovalAdaptor extends ArrayAdapter<RequisationForm> {
    private Context context;
    private Employee handledBy;
    private Icallback icallback;
     StringBuilder comments=  new StringBuilder();

    public ApprovalAdaptor(@NonNull Context context, int resource, @NonNull List<RequisationForm> objects, Employee handledBy) {
        super(context, resource, objects);
        this.context = context;
        this.handledBy = handledBy;
        this.icallback=(Icallback)context;
    }

    @NonNull
    @Override
    public View getView(final int position, @Nullable View view, @NonNull ViewGroup parent) {
        RequisationForm requisationForm = getItem(position);
        LayoutInflater inflater = (LayoutInflater) context.getSystemService(
                Activity.LAYOUT_INFLATER_SERVICE);
        view = inflater.inflate(R.layout.list_view_approval, null);
        if (requisationForm != null && view != null) {
            TextView tvempName = view.findViewById(R.id.tvempName);
            if (tvempName != null) {
                tvempName.setText(requisationForm.getRequestNumber());
            }
            TextView tvRequestNumber = view.findViewById(R.id.tvRequestNumber);
            if (tvRequestNumber != null) {
                tvRequestNumber.setText(requisationForm.getRequestedBy().getUserName());
            }
            TextView tvItemDetails = view.findViewById(R.id.itemDetail);
            if (tvItemDetails != null) {
                StringBuilder itemDetails = new StringBuilder();
                for (RequestItem requestItem : requisationForm.getRequestItems()) {
                    itemDetails.append(requestItem.getItem().getDescription())
                            .append(" - ")
                            .append(requestItem.getQuantity())
                            .append('\n');
                }

                tvItemDetails.setText(itemDetails);
            }

            Button btnAccept = view.findViewById(R.id.accept);
            Button btnReject = view.findViewById(R.id.reject);

            btnAccept.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View view) {

                    RequisationForm requisationForm1 = getItem(position);
                    requisationForm1.setFormStatus(2);
                    requisationForm1.setHandeledBy(handledBy);

                    alertDialog(requisationForm1);
                }
            });
            btnReject.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View view) {

                    RequisationForm requisationForm1 = getItem(position);
                    requisationForm1.setFormStatus(0);
                    requisationForm1.setHandeledBy(handledBy);

                    alertDialog(requisationForm1);
                }
            });

        }
        return view;
    }

    void alertDialog(RequisationForm requisationForm) {
        final RequisationForm rf = requisationForm;
        LayoutInflater li = LayoutInflater.from(getContext());
        View promptsView = li.inflate(R.layout.layout_comments, null);
        AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(getContext());
        alertDialogBuilder.setView(promptsView);
        final EditText userInput = promptsView.findViewById(R.id.etUserInput);

        alertDialogBuilder
                .setCancelable(false)
                .setPositiveButton("OK", new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int id) {
                        comments = new StringBuilder();
                        comments.append(userInput.getText());
                        rf.setComments(comments.toString());
                        postMethod(rf);
                    }
                })
                .setNegativeButton("Cancel", new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int id) {
                        dialog.cancel();
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

    }

    public void postMethod(RequisationForm rf) {

        RequestForm requestForm = new RequestForm();
        requestForm.setId(rf.getId());
        requestForm.setComments(rf.getComments());
        requestForm.setFormStatus(rf.getFormStatus());
        requestForm.setHandledBy(rf.getHandeledBy().getId());

        ApprovalPostAsyncTask approvalPostAsyncTask = new ApprovalPostAsyncTask();
        approvalPostAsyncTask.execute(requestForm);
        icallback.onRestart();
    }

    public interface Icallback{
        void onRestart();
    }
}

