package com.arun.api.Adaptor;

import android.app.Activity;
import android.content.Context;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import com.arun.api.Model.Retrieval;
import com.arun.api.R;

import java.util.List;
import java.util.Objects;

public class DepartmentAdapter extends ArrayAdapter<Retrieval> {
    private Context context;
    public List<Retrieval> lists;
    int updateActualQty = 0;

    public DepartmentAdapter(Context context, int resourceId, List<Retrieval> lists) {
        super(context, resourceId, lists);
        this.context = context;
        this.lists = lists;

    }

    public View getView(final int position, View view, ViewGroup parent) {
        final Retrieval rowItem = getItem(position);

        LayoutInflater inflater = (LayoutInflater) context.getSystemService(Activity.LAYOUT_INFLATER_SERVICE);
        view = inflater.inflate(R.layout.row_listview2, parent, false);

        EditText tvCount = view.findViewById(R.id.actualValue);
        Button btnIncrease = (Button) view.findViewById(R.id.actualDecrease);
        Button btnDecrease = (Button) view.findViewById(R.id.actualIncrease);

        ViewHolder viewHolder = new ViewHolder();
        viewHolder.deptCode = (TextView) view.findViewById(R.id.DepCode);
        viewHolder.ActualQty = (EditText) view.findViewById(R.id.actualValue);
        viewHolder.NeededQty = (TextView) view.findViewById(R.id.neededValue);
        viewHolder.btnPlus = (Button) view.findViewById(R.id.actualIncrease);
        final View finalView = view;
        viewHolder.btnPlus.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (rowItem.getActual() < rowItem.getNeeded()) {
                    updateActualQty = lists.get(position).Actual;
                    updateActualQty += 1;
                    View tempView = finalView;
                    actualQtydisplay(updateActualQty, tempView);
                    if (lists.contains(getItem(position))) {
                        rowItem.Actual = updateActualQty;
                    }
                }
            }
        });
        viewHolder.btnMinus = (Button) view.findViewById(R.id.actualDecrease);
        viewHolder.btnMinus.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                updateActualQty = lists.get(position).Actual;
                updateActualQty -= 1;
                View tempView = finalView;
                actualQtydisplay(updateActualQty, tempView);
                if (lists.contains(getItem(position))) {
                    rowItem.Actual = updateActualQty;
                }
            }
        });
        tvCount.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2) {

            }

            @Override
            public void onTextChanged(CharSequence charSequence, int i, int i1, int i2) {

            }

            @Override
            public void afterTextChanged(Editable editable) {
                if (!editable.toString().equalsIgnoreCase("")) {
                    if(Integer.parseInt(editable.toString())<=rowItem.getNeeded()){
                        rowItem.Actual = Integer.parseInt(editable.toString());

                    }else{
                        Toast.makeText(getContext(), "There is mismatch in value", Toast.LENGTH_SHORT).show();
                        actualQtydisplay(  rowItem.Actual , finalView);
                    }
                }
            }
        });
        if (rowItem != null && view != null) {
            TextView DepCode = view.findViewById(R.id.DepCode);
            if (DepCode != null) {
                DepCode.setText(rowItem.getDepartmentCode());
            }

            TextView Needed = view.findViewById(R.id.neededValue);
            if (Needed != null) {
                Needed.setText(String.valueOf(rowItem.getNeeded()));
            }

            EditText Actual = view.findViewById(R.id.actualValue);
            if (Actual != null) {
                Actual.setText(String.valueOf(rowItem.getActual()));
            }
        }

        return view;
    }

    void actualQtydisplay(int number, View view) {
        if(number<0){
            number=0;
        }
        EditText displayInteger = view.findViewById(R.id.actualValue);
        displayInteger.setText("" + number);
    }


    private class ViewHolder {
        TextView deptCode;
        TextView NeededQty;

        Button btnPlus;
        EditText ActualQty;
        Button btnMinus;
    }
}