import { Component, OnInit } from '@angular/core';
import { AdminLayoutComponent } from 'src/app/layouts/admin/admin-layout/admin-layout.component';

@Component({
  selector: 'app-dashboard-index',
  templateUrl: './dashboard-index.component.html',
  styleUrls: ['./dashboard-index.component.css']
})
export class DashboardIndexComponent implements OnInit {

  datadoughnut: any;
  datapie: any;
  databar: any;
  dataline: any;

  constructor(
    private adminLayoutComponent: AdminLayoutComponent,
  ) {

  }

  ngOnInit() {
    this.adminLayoutComponent.resetCounter();
    this.datadoughnut = {
      labels: ['A', 'B', 'C'],
      datasets: [
        {
          data: [300, 50, 100],
          backgroundColor: [
            '#4BC0C0',
            '#36A2EB',
            '#FFCE56'
          ],
          hoverBackgroundColor: [
            '#4BC0C0',
            '#36A2EB',
            '#FFCE56'
          ]
        }]
    };

    this.databar = {
      labels: ['1', '2', '3', '4', '5', '6', '7'],
      datasets: [
        {
          label: 'A',
          backgroundColor: '#42A5F5',
          borderColor: '#1E88E5',
          data: [65, 59, 80, 81, 56, 55, 40]
        },
        {
          label: 'B',
          backgroundColor: '#9CCC65',
          borderColor: '#7CB342',
          data: [28, 48, 40, 19, 86, 27, 90]
        }
      ]
    };

    this.dataline = {
      labels: ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran'],
      datasets: [
        {
          label: 'A',
          data: [65, 59, 80, 81, 56, 55],
          fill: false,
          borderColor: '#4bc0c0'
        },
        {
          label: 'B',
          data: [28, 48, 40, 19, 86, 1],
          fill: false,
          borderColor: '#565656'
        }
      ]
    };

    this.datapie = {
      labels: ['A', 'B', 'C'],
      datasets: [
        {
          data: [300, 50, 100],
          backgroundColor: [
            '#FF6384',
            '#36A2EB',
            '#FFCE56'
          ],
          hoverBackgroundColor: [
            '#FF6384',
            '#36A2EB',
            '#FFCE56'
          ]
        }]
    };
  }

}
