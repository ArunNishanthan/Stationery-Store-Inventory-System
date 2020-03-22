package com.arun.api.Adaptor;

import android.app.Activity;
import android.content.Context;
import android.icu.text.SimpleDateFormat;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;

import com.arun.api.AsyncTask.Post.RequisationPostAsyncTask;
import com.arun.api.Model.RequestItem;
import com.arun.api.Model.RequisitionByDepartmentItem;
import com.arun.api.R;

import java.text.ParseException;
import java.util.Date;
import java.util.List;

public class RequisationAdaptor extends ArrayAdapter<RequisitionByDepartmentItem> {
    private Context context;
    private Icallback icallback;

    public RequisationAdaptor(@NonNull Context context, int resource, @NonNull List<RequisitionByDepartmentItem> objects) {
        super(context, resource, objects);
        this.context = context;
        this.icallback = (Icallback) context;
    }

    @NonNull
    @Override
    public View getView(final int position, @Nullable View view, @NonNull ViewGroup parent) {
        final RequisitionByDepartmentItem requisitionByDepartmentItem = getItem(position);
        LayoutInflater inflater = (LayoutInflater) context.getSystemService(
                Activity.LAYOUT_INFLATER_SERVICE);

        view = inflater.inflate(R.layout.list_view_requisation, null);

        if (requisitionByDepartmentItem != null && view != null) {
            TextView tvDeptName = view.findViewById(R.id.deptName);
            if (tvDeptName != null) {
                tvDeptName.setText(requisitionByDepartmentItem.Department.getDepartmentName());
            }

            TextView tvlastUpdated = view.findViewById(R.id.lastUpdated);
            if (tvlastUpdated != null) {

                String pattern = "yyyy-MM-dd";
                String patternOut = "dd-MM-yyyy";
                SimpleDateFormat simpleDateFormat = new SimpleDateFormat(pattern);
                SimpleDateFormat simpleDateFormatOut = new SimpleDateFormat(patternOut);

                Date date = new Date();
                try {
                    date = simpleDateFormat.parse(requisitionByDepartmentItem.LastUpdated.substring(0, 10));
                } catch (ParseException e) {
                    e.printStackTrace();
                }
                tvlastUpdated.setText(simpleDateFormatOut.format(date));
            }

            TextView tvItemDetails = view.findViewById(R.id.itemDetail);
            if (tvItemDetails != null) {
                StringBuilder itemDetails = new StringBuilder();
                for (RequestItem requestItem : requisitionByDepartmentItem.RequestItems) {
                    itemDetails.append(requestItem.getItem().getDescription())
                            .append(" - ")
                            .append(requestItem.getQuantity())
                            .append('\n');
                }

                tvItemDetails.setText(itemDetails);
            }

            Button btnadd = view.findViewById(R.id.add);
            btnadd.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View view) {
                    RequisationPostAsyncTask requisationPostAsyncTask = new RequisationPostAsyncTask();
                    requisationPostAsyncTask.execute(requisitionByDepartmentItem.Department.getDepartmentCode());
                    icallback.onRestart();
                }
            });


        }
        return view;
    }

    public interface Icallback {
        void onRestart();
    }
}

