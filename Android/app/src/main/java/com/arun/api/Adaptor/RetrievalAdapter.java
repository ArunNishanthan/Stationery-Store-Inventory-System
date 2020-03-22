package com.arun.api.Adaptor;
import android.app.Activity;
import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.ListAdapter;
import android.widget.ListView;
import android.widget.TextView;

import com.arun.api.Model.RetrievalList;
import com.arun.api.R;

import java.util.List;

public class RetrievalAdapter extends ArrayAdapter<RetrievalList>
{
    private Context context;

    public RetrievalAdapter(Context context, int resourceId, List<RetrievalList> lists)
    {
        super(context, resourceId,lists);
        this.context= context;
    }

    public View getView(int position, View view, ViewGroup parent)
    {
        RetrievalList rowItem= getItem(position);

        LayoutInflater inflater= (LayoutInflater) context.getSystemService(Activity.LAYOUT_INFLATER_SERVICE);
        view = inflater.inflate(R.layout.row_listview, parent, false);

        if(rowItem!= null && view != null)
        {
            TextView textView1= view.findViewById(R.id.list_item1);
            if(textView1!= null)
            {
                textView1.setText(rowItem.getRetrievedItem());
            }

            TextView textView2= view.findViewById(R.id.list_item2);
            if(textView2!= null)
            {
                textView2.setText(String.valueOf(rowItem.getNeeded()));
            }

            TextView textView3= view.findViewById(R.id.list_item3);
            if(textView3!= null)
            {
                textView3.setText(String.valueOf(rowItem.getRetrieved()));
            }
            ListView lv = view.findViewById(R.id.listView2);
            if(lv!=null){
                DepartmentAdapter departmentAdapter = new DepartmentAdapter(getContext(),R.layout.row_listview2,rowItem.getRetrievals());
                lv.setAdapter(departmentAdapter);

                setListViewHeightBasedOnChildren(lv);
            }
        }
        return view;
    }



    public static void setListViewHeightBasedOnChildren(ListView listView) {
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

}
