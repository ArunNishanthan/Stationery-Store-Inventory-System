package com.arun.api.Adaptor;

import android.app.Activity;
import android.content.Context;
import android.system.ErrnoException;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Adapter;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ListAdapter;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;


import com.arun.api.Activities.DepDisbursementActivity;
import com.arun.api.Activities.RepresentativeActivity;
import com.arun.api.Adaptor.DeptDisbursementAdapter.IcallbackRestart;
import com.arun.api.AsyncTask.Get.DeptDisbursementItemRemoveAsyncTask;
import com.arun.api.Model.DisburseItem;
import com.arun.api.Model.Retrieval;
import com.arun.api.R;
import com.google.android.material.snackbar.Snackbar;

import java.util.List;

class ItemeAdapter extends ArrayAdapter<DisburseItem> {

    private Context context;
    int updateActualQty = 0;
    IcallbackRestart icallback;
    public List<DisburseItem> lists;
    int status;

    public ItemeAdapter(@NonNull Context context, int resource, @NonNull List<DisburseItem> lists, int status) {
        super(context, resource, lists);
        this.context = context;
        this.lists = lists;
        this.icallback = (IcallbackRestart) context;
        this.status = status;
    }

    @NonNull
    @Override
    public View getView(final int position, @Nullable View view, @NonNull ViewGroup parent) {

        final DisburseItem rowItem = getItem(position);
        LayoutInflater inflater = (LayoutInflater) context.getSystemService(Activity.LAYOUT_INFLATER_SERVICE);
        view = inflater.inflate(R.layout.row_listview4, parent, false);

        final View tempview = view;
        final ViewHolder viewHolder = new ViewHolder();
        viewHolder.itemDescription_textView = view.findViewById(R.id.itemDescription_textView);
        viewHolder.itemDeliveredEt = view.findViewById(R.id.delivered_editView);
        viewHolder.itemActualTv = view.findViewById(R.id.itemActual_textView);
        viewHolder.itemRetrievedTv = view.findViewById(R.id.retrieved_textView);
        viewHolder.btnMinus = view.findViewById(R.id.btnMinus);
        viewHolder.btnPlus = view.findViewById(R.id.btnPlus);
        viewHolder.btnRemoveItem = view.findViewById(R.id.btnremoveItem);

        viewHolder.btnRemoveItem.setVisibility(View.GONE);
        viewHolder.btnPlus.setVisibility(view.GONE);
        viewHolder.btnMinus.setVisibility(view.GONE);
        if (status == 3) {
            viewHolder.btnRemoveItem.setVisibility(View.VISIBLE);
            viewHolder.btnPlus.setVisibility(view.VISIBLE);
            viewHolder.btnMinus.setVisibility(view.VISIBLE);
        }


        if (rowItem != null && view != null) {
            if (viewHolder.itemDescription_textView != null) {
                viewHolder.itemDescription_textView.setText(rowItem.getRequestItem().getDescription());
            }
            if (viewHolder.itemDespTv != null) {
                viewHolder.itemDespTv.setText(rowItem.getRequestItem().getDescription());
            }
            if (viewHolder.itemActualTv != null) {
                viewHolder.itemActualTv.setText(String.valueOf(rowItem.getRequestedQuantity()));
            }
            if (viewHolder.itemRetrievedTv != null) {
                viewHolder.itemRetrievedTv.setText(String.valueOf(rowItem.getRetrivedQuantity()));
            }
            if (viewHolder.itemDeliveredEt != null) {
                viewHolder.itemDeliveredEt.setText(String.valueOf(rowItem.getDisbursedQuantity()));
            }
        }
        viewHolder.btnRemoveItem.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                DeptDisbursementItemRemoveAsyncTask deptDisbursementItemRemoveAsyncTask = new DeptDisbursementItemRemoveAsyncTask();
                deptDisbursementItemRemoveAsyncTask.execute(String.valueOf(rowItem.DisburseItemID));
                lists.remove(rowItem);
                Snackbar.make(view, "Removed", Snackbar.LENGTH_SHORT)
                        .show();
                icallback.onRestart();
            }
        });
        viewHolder.btnPlus.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                updateActualQty = rowItem.getDisbursedQuantity();
                if (rowItem.getRetrivedQuantity() == updateActualQty) {
                    Toast.makeText(getContext(), "Enough disburse Quantity.", Toast.LENGTH_SHORT).show();
                } else {
                    updateActualQty += 1;
                    actualQtydisplay(updateActualQty, tempview);
                    rowItem.setDisbursedQuantity(updateActualQty);
                }
            }
        });

        viewHolder.btnMinus.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                updateActualQty = rowItem.getDisbursedQuantity();
                updateActualQty -= 1;
                actualQtydisplay(updateActualQty, tempview);
                rowItem.setDisbursedQuantity(updateActualQty);
            }
        });


        if (this.context instanceof RepresentativeActivity) {
            viewHolder.btnMinus.setVisibility(View.GONE);
            viewHolder.btnPlus.setVisibility(View.GONE);
            viewHolder.btnRemoveItem.setVisibility(View.GONE);
            viewHolder.itemDeliveredEt.setVisibility(View.GONE);
            view.findViewById(R.id.delivered_label).setVisibility(View.GONE);
        }
        return view;
    }

    public class ViewHolder {

        Button btnPlus, btnMinus, btnRemoveItem;
        TextView itemDespTv, itemActualTv, itemRetrievedTv, itemDescription_textView, itemDeliveredEt;
    }

    void actualQtydisplay(int number, View tempview) {
        TextView displayInteger = tempview.findViewById(R.id.delivered_editView);
        if (number < 0) {
            Toast.makeText(getContext(), "Quantity not lower than Zero.", Toast.LENGTH_SHORT).show();
            number = 0;
        }
        displayInteger.setText("" + number);
    }


}
