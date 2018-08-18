using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultTheme
{
    // ������� ��������������� ����� - ����� ����������� ��������������� ������� ����� �������� ����������� ������������ ��������� �����

    // ����� - ����� ��� �������� �� ���������� �������
    public class LeftRightMovedDefault : Platform
    {
        public float SpeedMoving;
        
        public Vector3 Forvard; // ����� ������ ������� ����� ��������, �� �� ����� ���������

        public bool ForvardVectorGoingRight;

        protected override void DoStart()
        {
            base.DoStart();

            SpeedMoving = 400;

            Forvard = new Vector3(200, 0, 0);
            ForvardVectorGoingRight = true;
        }

        protected override void DoUpdate()
        {
            base.DoUpdate();

            MoveLeftRight();
        }

        //void OnCollisionEnter(Collision collision)
        void OnCollisionEnter()
        {
            // ��� �������� ����, ����� � ����� �������� � ��������� ������� � ������� ��������������� �����, � ��� ������ ��������� � � ����� ������� �� ��������, ����� ���������

            SwapForvardVectorGoingRight();
            MoveLeftRight();
        }

        void MoveLeftRight()
        {
            Vector3 forvard = ForvardVectorGoingRight ? Forvard : -Forvard;

            Vector3 curVec = forvard.normalized * Time.deltaTime * SpeedMoving;

            transform.position += curVec;

            // ���� ���������� ����� ��������� � ������� �������� ������ ����������� - ������ �����������
            if( (transform.position-StartPosition).magnitude > forvard.magnitude ) {
                Vector3 offset = transform.position - StartPosition - forvard;

                // ���� � ������ �� ������� � deltatime ������ ��������� ��� ��������� ������ ������ � ��������������� �������, ������������ � ������� - ������� ��������������� �����
                if ( offset.magnitude > 2*forvard.magnitude )
                    transform.position = - ( StartPosition + forvard );
                else
                    // ��� ��� ������ �� ��������� �� ���������� ������ �� ���������� offset, � ���� ���� ����� ���� ������ �� ���������� offset
                    transform.position -= 2*offset;

                // ������ �����������
                SwapForvardVectorGoingRight();
            }
        }

        void SwapForvardVectorGoingRight()
        {
            ForvardVectorGoingRight = !ForvardVectorGoingRight;
        }
    }
}