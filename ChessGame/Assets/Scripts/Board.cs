using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ������ �����
/// </summary>
public class Board : MonoBehaviour
{

    Movement movement; 
    // Start is called before the first frame update
    void Start()
    {
        movement = new Movement();
    }

    // Update is called once per frame
    void Update()
    {
        movement.move();   
    }

    class Movement
    {
        /// <summary>
        /// ����� ������ ����� ��������� � ����� ����� �����
        /// </summary>
        int dop_chislo;
        /// <summary>
        /// ������� ��������� ����
        /// </summary>
        State state;

        /// <summary>
        /// ��������� ���������
        /// </summary>
        State next_state;

        /// <summary>
        /// ��������� ������
        /// </summary>
        GameObject item;

        GameObject selected_figure = GameObject.Find("selected_figure");

        /// <summary>
        /// ��������� ������ ������
        /// </summary>
        Vector2 w5;

        /// <summary>
        /// ��������� ������� �����
        /// </summary>
        Vector2 b5;

        /// <summary>
        /// ��� ���
        /// </summary>
        bool move_tern;

        /// <summary>
        /// ���� �������� ������
        /// </summary>
        bool wkm, bkm;

        /// <summary>
        /// ���� �������� �����
        /// </summary>
        bool wsrm, wlrm, bsrm, blrm;

        /// <summary>
        /// ������� �����
        /// </summary>
        List<string> move_history = new List<string>();

        /// <summary>
        /// ���� ���������� ������ �� ������� ��� ����� �����
        /// </summary>
        bool[] w_pawns = new bool[8] { false, false, false, false, false, false, false, false };

        /// <summary>
        /// ���� ���������� ������ �� ������� ��� ������ �����
        /// </summary>
        bool[] b_pawns = new bool[8] { false, false, false, false, false, false, false, false };

        /// <summary>
        /// ���� ����, ��� ���� ������ �� �������
        /// </summary>
        short taking_on_the_pass = 0;

        /// <summary>
        /// ����������� ������
        /// </summary>
        public Movement() 
        {
            dop_chislo = 1;
            next_state = State.drag;
            state = State.none;
            w5 = new Vector2(1, -6);
            b5 = new Vector2(1, 8);
            move_tern = false;
            wkm = true;
            bkm = true;
            wsrm = true;
            bsrm = true;
            wlrm = true;
            blrm = true;
            item = null;

            //Vector2 sf_pos = new Vector2(-15, -5);
            //selected_figure = GetItemAt(sf_pos);// Physics2D.RaycastAll(new Vector2(-15f, -5f), new Vector2(-15f, -5f), 1f)[0].transform;
            //selected_figure 
            //Debug.Log(selected_figure.name);
        }


        /// <summary>
        /// ���������� ����
        /// </summary>
        public void move()
        {
            switch (state)
            {
                case State.none:
                    if (isMouseActionPresed())
                        pickup();
                    break;
                case State.waiting:
                    wait();
                    break;
                case State.drag:
                    if (isMouseActionPresed())
                        drag();
                    break;
            }
        }


        /// <summary>
        /// �������� ��������� �������
        /// </summary>
        /// <returns></returns>
        Vector2 getClickPosition()
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }


        /// <summary>
        /// �������� ������ �� ���������� (��� null ���� ������ �� ���������� ���)
        /// </summary>
        /// <param name="position">���������� ���������� ������</param>
        /// <returns></returns>
        Transform GetItemAt(Vector2 position)
        {
            RaycastHit2D[] figures = Physics2D.RaycastAll(position, position, 0.5f);
            if (figures.Length == 0)
                return null;
            return figures[0].transform;
        }


        /// <summary>
        /// ������ ���� (����� ������ ��� ���� � ����� ��� ������������� ������ ���� ������)
        /// </summary>
        void pickup() 
        {
            Vector2 click_position = getClickPosition();
            Transform clicked_item = GetItemAt(click_position);

            if (clicked_item == null)
                return;

            if ((move_tern && clicked_item.name[0] == 'w') || (!move_tern && clicked_item.name[0] == 'b')) 
                return;
            
            state = State.waiting;
            item = clicked_item.gameObject;

            selected_figure.transform.position = new Vector3(item.transform.position.x, item.transform.position.y, 1);

            Debug.Log("Picked " + item.name);
        }


        /// <summary>
        /// ������������� ��������� ����� none � drag ������� ���� ���� �������� ���
        /// </summary>
        void wait()
        {
            if (!isMouseActionPresed())
            {
                state = next_state;
                if (next_state == State.drag)
                    next_state = State.none;
                else
                    next_state = State.drag;
            }

        }


        /// <summary>
        /// ��� ��������� �������������� ����������� ����� ������ ����� ������� ����� � ����
        /// </summary>
        void drag()
        {
            Vector2 item_position = new Vector2(item.transform.position.x, item.transform.position.y);                                                                                                  //
            Vector2 click_position = getClickPosition();                                                                                                                                                //
            click_position = new Vector2(click_position.x - click_position.x % 2 + click_position.x / Mathf.Abs(click_position.x), click_position.y + (click_position.y/Mathf.Abs(click_position.y)));  //
            click_position = new Vector2(click_position.x, click_position.y - click_position.y%2);                                                                                                      //��� �������� ��������� � ������ ������

            if (click_position.x >=-7 && click_position.x <=7 && click_position.y >= -6 && click_position.y <= 8)   // ���� ���� ���������� � �������� �������� ����(��� ���������� UI ��� ������� ��� �������)
            { 
                Transform clicked_item = GetItemAt(click_position);
                if (clicked_item != null)   // �� �������� ��� �� ����� ������ ��� �� �����
                {
                    if (move_tern)  //��������� ��� ���
                    {
                        if (clicked_item.name[0] == 'w')    // �������� �� �����
                        {
                            if (!getPremissionToMove(click_position)) return;

                            selected_figure.transform.position = new Vector3(-15, -5, 1);
                            Destroy(clicked_item.gameObject);

                            item.transform.position = new Vector3(click_position.x, click_position.y, 0);
                            state = State.waiting;
                            if (move_tern) move_tern = false;
                            else move_tern = true;
                        }
                        else //�������� �� �����
                        {
                            item = clicked_item.gameObject;
                            selected_figure.transform.position = new Vector3(item.transform.position.x, item.transform.position.y, 1);
                        }
                    }
                    else
                    { 
                        if (clicked_item.name[0] == 'b')    // �������� �� �����
                        {
                            if (!getPremissionToMove(click_position)) return;
                            string move_code = transformCodeToFegurName(item.name[1]) + transformPointerToPositionName(item.transform.position) + 'x' + transformPointerToPositionName(click_position);

                            selected_figure.transform.position = new Vector3(-15, -5, 1);
                            Destroy(clicked_item.gameObject);

                            move_history.Add(move_code);
                            Debug.Log(move_code);
                            item.transform.position = new Vector3(click_position.x, click_position.y, 0);
                            if (getPremissionToChenge(item))
                            {
                                chengePeshku(item, move_tern);
                            }
                            state = State.waiting;
                                if (move_tern) move_tern = false;
                                else move_tern = true;
                        }
                        else    //�������� �� �����
                        {
                            item = clicked_item.gameObject;
                            selected_figure.transform.position = new Vector3(item.transform.position.x, item.transform.position.y, 1);
                        }
                    }
                }
                else //�������� �� ������ ������
                {
                    if (!getPremissionToMove(click_position))

                        return;


                    selected_figure.transform.position = new Vector3(-15, -5, 1);
                    string move_code;

                    if (taking_on_the_pass == 1 || taking_on_the_pass == 2)
                    {
                        move_code = transformCodeToFegurName(item.name[1]) + transformPointerToPositionName(item.transform.position) + 'x' + transformPointerToPositionName(click_position);

                        Vector2 position_ataced_pawn;
                        if (taking_on_the_pass == 1)
                            position_ataced_pawn = new Vector2(click_position.x, click_position.y - 2);
                        else
                            position_ataced_pawn = new Vector2(click_position.x, click_position.y + 2);

                        Transform ataced_pawn = GetItemAt(position_ataced_pawn);

                        Destroy(ataced_pawn.gameObject);
                        move_history.Add(move_code);
                        Debug.Log(move_code);

                        taking_on_the_pass = 0;

                        item.transform.position = new Vector3(click_position.x, click_position.y, 0);


                        state = State.waiting;

                        if (move_tern)
                            move_tern = false;
                        else
                            move_tern = true;

                        selected_figure.transform.position = new Vector3(-15, -5, 1);
                        return;
                    }

                    move_code = transformCodeToFegurName(item.name[1]) + transformPointerToPositionName(item.transform.position) + '-' + transformPointerToPositionName(click_position);
                    move_history.Add(move_code);
                    Debug.Log(move_code);
                    item.transform.position = new Vector3(click_position.x, click_position.y, 0);
                    if (getPremissionToChenge(item))
                    {
                        Debug.Log("���� ����������");
                        chengePeshku(item, move_tern);
                    }
                    state = State.waiting;

                    if (move_tern) 
                        move_tern = false;
                    else 
                        move_tern = true;
                }
            }

        }

        void chengePeshku(GameObject go, bool movetern)
        {
            if (!movetern)
            {
                Debug.Log("������ ���");
                ChengeBoxW.SelectFigur(go, dop_chislo);
                dop_chislo++;
            }
            else
            {
                ChengeBoxB.SelectFigur(go, dop_chislo);
                dop_chislo++;
            }
        }

        bool getPremissionToChenge(GameObject go)
        {
            if (go.name[0] == 'w' && go.name[1] == '4' && go.transform.position.y == 8) return true;
            if (go.name[0] == 'b' && go.name[1] == '4' && go.transform.position.y == -6) return true;
            return false;
        }


        /// <summary>
        /// ������ ����������� ����� �� ���������� ������ (�� ��� ����� � item) ������� ��� � ��������� ������
        /// </summary>
        /// <param name="move_position">��������� ������ ���� ����� ������ </param>
        /// <returns></returns>
        bool getPremissionToMove( Vector2 move_position)
        {
            string item_name = item.name;

            Vector2 item_position = new Vector2(item.transform.position.x, item.transform.position.y);
            Vector2 king_position;

            if (!move_tern) 
            { 
                king_position = w5; 
            }
            else 
            { 
                king_position = b5; 
            }

            switch (item_name[1])
            { 
                case '1'://�����
                    if (item_position.x == move_position.x || item_position.y == move_position.y)
                    {
                        if (move_position.x - item_position.x == 0)
                        {
                            item_position.y += (move_position.y - item_position.y) * 2 / Mathf.Abs(move_position.y - item_position.y);
                            while (move_position != item_position)
                            {
                                if (GetItemAt(item_position) != null) return false;//���� ���������� ������ �������
                                item_position.y += (move_position.y - item_position.y) * 2 / Mathf.Abs(move_position.y - item_position.y);
                            }
                            if (checkAtack(king_position, item_position, move_position)) return false;//������ ����� ��� �����
                            if (!move_tern)                                                                 //
                            {                                                                               //
                                if (wsrm && item_position.x == 7 && item_position.y == -6) wsrm = false;    //
                                if (wlrm && item_position.x == -7 && item_position.y == -6) wlrm = false;   //
                            }                                                                               //
                            else
                            {                                                                               //
                                if (bsrm && item_position.x == 7 && item_position.y == 8) bsrm = false;     //
                                if (blrm && item_position.x == -7 && item_position.y == 8) blrm = false;    //
                            }                                                                               //��������� ����� ��� ����� ����� ���� ������
                            return true;
                        }
                        else 
                        {
                            item_position.x += (move_position.x - item_position.x) * 2 / Mathf.Abs(move_position.x - item_position.x);
                            while (move_position != item_position)
                            {
                                if (GetItemAt(item_position) != null) return false;
                                item_position.x += (move_position.x - item_position.x) * 2 / Mathf.Abs(move_position.x - item_position.x);
                            }
                            if (checkAtack(king_position, item_position, move_position)) return false;
                            if (!move_tern)                                                                 //
                            {                                                                               //
                                if (wsrm && item_position.x == 7 && item_position.y == -6) wsrm = false;    //
                                if (wlrm && item_position.x == -7 && item_position.y == -6) wlrm = false;   //
                            }                                                                               //
                            else
                            {                                                                               //
                                if (bsrm && item_position.x == 7 && item_position.y == 8) bsrm = false;     //
                                if (blrm && item_position.x == -7 && item_position.y == 8) blrm = false;    //
                            }                                                                               //��������� ��������� ����� ��� �����
                            return true;
                        }
                    }
                    return false;
                case '2'://����
                    if (Mathf.Abs((move_position.x - item_position.x) / (move_position.y - item_position.y)) == 1)
                    {
                        item_position.x += (move_position.x - item_position.x) * 2 / Mathf.Abs(move_position.x - item_position.x);
                        item_position.y += (move_position.y - item_position.y) * 2 / Mathf.Abs(move_position.y - item_position.y);
                        while (move_position != item_position)
                        {
                            if (GetItemAt(item_position) != null) return false; //���� ���������� ������ �������
                            item_position.x += (move_position.x - item_position.x) * 2 / Mathf.Abs(move_position.x - item_position.x);
                            item_position.y += (move_position.y - item_position.y) * 2 / Mathf.Abs(move_position.y - item_position.y);
                        }
                        if (checkAtack(king_position, item_position, move_position)) return false;//������ ����� ��� �����
                        return true;
                    }
                    return false;
                case '3'://����
                    switch (move_position.y - item_position.y)
                    {
                        case 4:
                            if (Mathf.Abs(move_position.x-item_position.x) == 2 && !checkAtack(king_position, item_position, move_position)) return true;
                            return false;
                        case 2:
                            if (Mathf.Abs(move_position.x - item_position.x) == 4 && !checkAtack(king_position, item_position, move_position)) return true;
                            return false;
                        case -2:
                            if (Mathf.Abs(move_position.x - item_position.x) == 4 && !checkAtack(king_position, item_position, move_position)) return true;
                            return false;
                        case -4:
                            if (Mathf.Abs(move_position.x - item_position.x) == 2 && !checkAtack(king_position, item_position, move_position)) return true;
                            return false;
                        default:
                            return false;

                    }
                case '4':   //�����
                    if (item_name[0] == 'w')
                    {
                        if (move_position.y - item_position.y == 2)
                        {
                            if (move_position.x != item_position.x)
                            {
                                //����� ������
                                if (Mathf.Abs(move_position.x - item_position.x) == 2 && GetItemAt(move_position) != null && !checkAtack(king_position, item_position, move_position))
                                { 
                                    w_pawns[(int)char.GetNumericValue(item_name[3])] = false;
                                    return true; 
                                }

                                //������ �� �������
                                Vector2 position_ataced_pawn = new Vector2(move_position.x, move_position.y - 2);
                                Transform ataced_pawn = GetItemAt(position_ataced_pawn);
                                if (GetItemAt(move_position) == null && ataced_pawn.name[0] == 'b' && ataced_pawn.name[1] == '4' && b_pawns[(int)char.GetNumericValue(ataced_pawn.name[3])] == true && !checkAtack(king_position, item_position, move_position) && Mathf.Abs(move_position.x - item_position.x) == 2)
                                {
                                    w_pawns[(int)char.GetNumericValue(item_name[3])] = false;
                                    taking_on_the_pass = 1;
                                    return true;
                                }
                                return false;
                            }
                            Debug.Log(!checkAtack(king_position, item_position, move_position));
                            //��� �� ���� ������ ������
                            if (GetItemAt(move_position) == null && !checkAtack(king_position, item_position, move_position))
                            {
                                Debug.Log((int)char.GetNumericValue(item_name[3]));
                                w_pawns[(int)char.GetNumericValue(item_name[3])] = false; // w4_1, ��� 1 - ����� �����
                                return true;    
                            }
                            return false;
                        }

                        //������ ��� ����� �� 2 ������
                        if (move_position.y - item_position.y == 4 && GetItemAt(move_position) == null && item_position.y == -4 && move_position.x - item_position.x == 0 && !checkAtack(king_position, item_position, move_position))
                        {
                            Debug.Log((int)char.GetNumericValue(item_name[3]));
                            w_pawns[(int)char.GetNumericValue(item_name[3])] = true;
                            return true;
                        }
                        return false;
                    }
                    else 
                    {
                        if (move_position.y - item_position.y == -2)
                        {
                            if (move_position.x != item_position.x)
                            {
                                //����� ������
                                if (Mathf.Abs(move_position.x - item_position.x) == 2 && GetItemAt(move_position) != null && !checkAtack(king_position, item_position, move_position))
                                {
                                    b_pawns[(int)char.GetNumericValue(item_name[3])] = false;
                                    return true;
                                }

                                //������ �� �������
                                Vector2 position_ataced_pawn = new Vector2(move_position.x, move_position.y + 2);
                                Transform ataced_pawn = GetItemAt(position_ataced_pawn);
                                if (GetItemAt(move_position) == null && ataced_pawn.name[0] == 'w' && ataced_pawn.name[1] == '4' && w_pawns[(int)char.GetNumericValue(ataced_pawn.name[3])] == true && !checkAtack(king_position, item_position, move_position) && Mathf.Abs(move_position.x - item_position.x) == 2)
                                {
                                    b_pawns[(int)char.GetNumericValue(item_name[3])] = false;
                                    taking_on_the_pass = 2;
                                    return true;
                                }
                                return false;
                            }

                            
                            //��� �� ���� ������ ������
                            if (GetItemAt(move_position) == null && !checkAtack(king_position, item_position, move_position))
                            {
                                b_pawns[(int)char.GetNumericValue(item_name[3])] = false;
                                return true;
                            }

                            return false;
                        }

                        //������ ��� ����� �� 2 ������
                        if (move_position.y - item_position.y == -4 && GetItemAt(move_position) == null && item_position.y == 6 && move_position.x - item_position.x == 0 && !checkAtack(king_position, item_position, move_position))
                        {
                            b_pawns[(int)char.GetNumericValue(item_name[3])] = true;
                            return true;
                        }
                        return false;
                    }
                case '6'://�����
                    if (item_position.x == move_position.x || item_position.y == move_position.y)// ��� ���������� ����� ����� � �����
                    {
                        if (move_position.x - item_position.x == 0)
                        {
                            item_position.y += (move_position.y - item_position.y) * 2 / Mathf.Abs(move_position.y - item_position.y);
                            while (move_position != item_position)
                            {
                                if (GetItemAt(item_position) != null) return false;
                                item_position.y += (move_position.y - item_position.y) * 2 / Mathf.Abs(move_position.y - item_position.y);
                            }
                            if (checkAtack(king_position, item_position, move_position)) return false;
                            return true;
                        }
                        else
                        {
                            item_position.x += (move_position.x - item_position.x) * 2 / Mathf.Abs(move_position.x - item_position.x);
                            while (move_position != item_position)
                            {
                                if (GetItemAt(item_position) != null) return false;
                                item_position.x += (move_position.x - item_position.x) * 2 / Mathf.Abs(move_position.x - item_position.x);
                            }
                            if (checkAtack(king_position, item_position, move_position)) return false;
                            return true;
                        }
                    }
                    else if (Mathf.Abs((move_position.x - item_position.x) / (move_position.y - item_position.y)) == 1)
                    {
                        item_position.x += (move_position.x - item_position.x) * 2 / Mathf.Abs(move_position.x - item_position.x);
                        item_position.y += (move_position.y - item_position.y) * 2 / Mathf.Abs(move_position.y - item_position.y);
                        while (move_position != item_position)
                        {
                            if (GetItemAt(item_position) != null) return false;
                            item_position.x += (move_position.x - item_position.x) * 2 / Mathf.Abs(move_position.x - item_position.x);
                            item_position.y += (move_position.y - item_position.y) * 2 / Mathf.Abs(move_position.y - item_position.y);
                        }
                        if (checkAtack(king_position, item_position, move_position)) return false;
                        return true;
                    }
                    return false;
                case '5'://������
                    if (Mathf.Abs(move_position.x - item_position.x) <= 2 && Mathf.Abs(move_position.y - item_position.y) <= 2 && !checkAtack(move_position, item))//��� ������ �� 1 ������
                    {
                        if (!move_tern) { w5 = move_position; wkm = false; }
                        else { b5 = move_position; bkm = false; }
                        return true;
                    }
                    else//�������� � �����������
                    {
                        if (!move_tern)
                        {
                            if (move_position.x == 5 && move_position.y == -6 && wsrm && wkm && GetItemAt(new Vector2(5, -6)) == null && GetItemAt(new Vector2(3, -6)) == null && !checkAtack(new Vector2(5, -6)) && !checkAtack(new Vector2(3, -6)) && !checkAtack(new Vector2(1, -6)))
                            {
                                GetItemAt(new Vector2(7, -6)).position = new Vector3(3, -6, 0);
                                wkm = false;
                                wsrm = false;
                                w5 = move_position;
                                return true;
                            }
                            if (move_position.x == -3 && move_position.y == -6 && wlrm && wkm && GetItemAt(new Vector2(-1, -6)) == null && GetItemAt(new Vector2(-3, -6)) == null && GetItemAt(new Vector2(-5, -6)) == null && !checkAtack(new Vector2(-1, -6)) && !checkAtack(new Vector2(-3, -6)) && !checkAtack(new Vector2(1, -6)))
                            {
                                GetItemAt(new Vector2(-7, -6)).position = new Vector3(-1, -6, 0);
                                wkm = false;
                                wlrm = false;
                                w5 = move_position;
                                return true;
                            }
                        }
                        else
                        {
                            if (move_position.x == 5 && move_position.y == 8 && bsrm && bkm && GetItemAt(new Vector2(5, 8)) == null && GetItemAt(new Vector2(3, 8)) == null && !checkAtack(new Vector2(5, 8)) && !checkAtack(new Vector2(3, 8)) && !checkAtack(new Vector2(1, 8)))
                            {
                                GetItemAt(new Vector2(7, 8)).position = new Vector3(3, 8, 0);
                                bkm = false;
                                bsrm = false;
                                b5 = move_position;
                                return true;
                            }
                            if (move_position.x == -3 && move_position.y == 8 && blrm && bkm && GetItemAt(new Vector2(-1, 8)) == null && GetItemAt(new Vector2(-3, 8)) == null && GetItemAt(new Vector2(-5, 8)) == null && !checkAtack(new Vector2(-1, 8)) && !checkAtack(new Vector2(-3, 8)) && !checkAtack(new Vector2(1, 8)))
                            {
                                GetItemAt(new Vector2(-7, 8)).position = new Vector3(-1, 8, 0);
                                bkm = false;
                                blrm = false;
                                b5 = move_position;
                                return true;
                            }
                        }
                    }
                    return false;
                default: return false;
            }
        }


        /// <summary>
        /// ������� �������� ��������� ����� ������� (�����, �����, ������ � ��) � ������������� ������
        /// </summary>
        /// <param name="code">������ ��������� (1-�����, 2-����, 3-����, 4-�����, 5-������, 6-�����) </param>
        /// <returns></returns>
        char transformCodeToFegurName(char code)
        {
            switch (code)
            {
                case '1':
                    return 'R';
                case '2':
                    return 'B';
                case '3':
                    return 'N';
                case '5':
                    return 'K';
                case '6':
                    return 'Q';
                default:
                    return 'p';
            }
        }


        /// <summary>
        /// ������ ����������� ��� �������� ��������� ������
        /// </summary>
        /// <param name="check_item_position">��������� ������</param>
        /// <returns></returns>
        bool checkAtack(Vector2 check_item_position)//���� ����� ���� � ������ �� ��� while ������������ �������� ����� ����� ����� � ����� � if-� � ����� ���� ����� � �����
        {
            Vector2 check_position = new Vector2 (check_item_position.x, check_item_position.y+2);
            Transform encountered_item;
            while (check_position.y <= 8)
            {
                encountered_item = GetItemAt(check_position);
                if (encountered_item != null)
                {
                    if (!move_tern)
                    {
                        if (encountered_item.name[0] == 'w') break;
                        if (encountered_item.name[1] == '1') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                    else
                    {
                        if (encountered_item.name[0] == 'b') break;
                        if (encountered_item.name[1] == '1') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                    
                }
                check_position.y += 2;
            }
            check_position.y = check_item_position.y - 2;
            while (check_position.y >= -6)
            {
                encountered_item = GetItemAt(check_position);
                if (encountered_item != null)
                {
                    if (!move_tern)
                    {
                        if (encountered_item.name[0] == 'w') break;
                        if (encountered_item.name[1] == '1') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                    else
                    {
                        if (encountered_item.name[0] == 'b') break;
                        if (encountered_item.name[1] == '1') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }

                }
                check_position.y -= 2;
            }
            check_position.y = check_item_position.y;
            check_position.x = check_item_position.x + 2;
            while (check_position.x <= 7)
            {
                encountered_item = GetItemAt(check_position);
                if (encountered_item != null)
                {
                    if (!move_tern)
                    {
                        if (encountered_item.name[0] == 'w') break;
                        if (encountered_item.name[1] == '1') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                    else
                    {
                        if (encountered_item.name[0] == 'b') break;
                        if (encountered_item.name[1] == '1') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }

                }
                check_position.x += 2;
            }
            check_position.x = check_item_position.x - 2;
            while (check_position.x >= -7)
            {
                encountered_item = GetItemAt(check_position);
                if (encountered_item != null)
                {
                    if (!move_tern)
                    {
                        if (encountered_item.name[0] == 'w') break;
                        if (encountered_item.name[1] == '1') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                    else
                    {
                        if (encountered_item.name[0] == 'b') break;
                        if (encountered_item.name[1] == '1') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }

                }
                check_position.x -= 2;
            }
            check_position.x = check_item_position.x + 2;
            check_position.y = check_item_position.y + 2;
            while (check_position.y <= 8 && check_position.x <= 7)
            {
                encountered_item = GetItemAt(check_position);
                if (encountered_item != null)
                {
                    if (!move_tern)
                    {
                        if (encountered_item.name[0] == 'w') break;
                        if (encountered_item.name[1] == '2') return true;
                        if (encountered_item.name[1] == '6') return true;
                    }
                    else
                    {
                        if (encountered_item.name[0] == 'b') break;
                        if (encountered_item.name[1] == '2') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                }
                check_position.x += 2;
                check_position.y += 2;
            }
            check_position.x = check_item_position.x - 2;
            check_position.y = check_item_position.y - 2;
            while (check_position.y >= -6 && check_position.x >= -7)
            {
                encountered_item = GetItemAt(check_position);
                if (encountered_item != null)
                {
                    if (!move_tern)
                    {
                        if (encountered_item.name[0] == 'w') break;
                        if (encountered_item.name[1] == '2') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                    else
                    {
                        if (encountered_item.name[0] == 'b') break;
                        if (encountered_item.name[1] == '2') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                }
                check_position.x -= 2;
                check_position.y -= 2;
            }
            check_position.x = check_item_position.x + 2;
            check_position.y = check_item_position.y - 2;
            while (check_position.y >= -6 && check_position.x <= 7)
            {
                encountered_item = GetItemAt(check_position);
                if (encountered_item != null)
                {
                    if (!move_tern)
                    {
                        if (encountered_item.name[0] == 'w') break;
                        if (encountered_item.name[1] == '2') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                    else
                    {
                        if (encountered_item.name[0] == 'b') break;
                        if (encountered_item.name[1] == '2') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                }
                check_position.x += 2;
                check_position.y -= 2;
            }
            check_position.x = check_item_position.x - 2;
            check_position.y = check_item_position.y + 2;
            while (check_position.y <= 8 && check_position.x >= -7)
            {
                encountered_item = GetItemAt(check_position);
                if (encountered_item != null)
                {
                    if (!move_tern)
                    {
                        if (encountered_item.name[0] == 'w') break;
                        if (encountered_item.name[1] == '2') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                    else
                    {
                        if (encountered_item.name[0] == 'b') break;
                        if (encountered_item.name[1] == '2') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                }
                check_position.x -= 2;
                check_position.y += 2;
            }
            if (!move_tern)
            {
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 2, check_item_position.y + 4));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 4, check_item_position.y + 2));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 2, check_item_position.y - 4));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 4, check_item_position.y - 2));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 2, check_item_position.y + 4));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 4, check_item_position.y + 2));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 2, check_item_position.y - 4));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 4, check_item_position.y - 2));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 2, check_item_position.y + 2));
                if (encountered_item != null && encountered_item.name[0] == 'b' && (encountered_item.name[1] == '4' || encountered_item.name[1] == '5')) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 2, check_item_position.y + 2));
                if (encountered_item != null && encountered_item.name[0] == 'b' && (encountered_item.name[1] == '4' || encountered_item.name[1] == '5')) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x, check_item_position.y + 2));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '5') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 2, check_item_position.y));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '5') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 2, check_item_position.y));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '5') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 2, check_item_position.y - 2));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '5') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 2, check_item_position.y - 2));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '5') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x, check_item_position.y - 2));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '5') return true;
            }
            else 
            {
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 2, check_item_position.y + 4));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 4, check_item_position.y + 2));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 2, check_item_position.y - 4));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 4, check_item_position.y - 2));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 2, check_item_position.y + 4));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 4, check_item_position.y + 2));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 2, check_item_position.y - 4));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 4, check_item_position.y - 2));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 2, check_item_position.y - 2));
                if (encountered_item != null && encountered_item.name[0] == 'w' && (encountered_item.name[1] == '4' || encountered_item.name[1] == '5')) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 2, check_item_position.y - 2));
                if (encountered_item != null && encountered_item.name[0] == 'w' && (encountered_item.name[1] == '4' || encountered_item.name[1] == '5')) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x, check_item_position.y - 2));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '5') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 2, check_item_position.y));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '5') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 2, check_item_position.y));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '5') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 2, check_item_position.y + 2));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '5') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 2, check_item_position.y + 2));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '5') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x, check_item_position.y + 2));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '5') return true;
            }
            return false;
        }


        /// <summary>
        /// ������ ����������� ����� �� ����������� ������ ��� ������ ����� ���� ������ �� ������� 1 � ������� 2
        /// </summary>
        /// <param name="check_item_position">����������� ������ </param>
        /// <param name="exception_position_first">��������� ������ ������� ������ </param>
        /// <param name="ex�eption_position_second">�������� ������ ������� ������</param>
        /// <returns></returns>
        bool checkAtack(Vector2 check_item_position, Vector2 exception_position_first ,Vector2 ex�eption_position_second)
        {
            Vector2 check_position = new Vector2(check_item_position.x, check_item_position.y + 2);
            Transform encountered_item;
            while (check_position.y <= 8)
            {
                encountered_item = GetItemAt(check_position);
                if ((encountered_item != null && check_position != exception_position_first) || check_position == ex�eption_position_second)
                {
                    if (!move_tern)
                    {
                        if (check_position==ex�eption_position_second) break;
                        if (encountered_item.name[0] == 'w') break;
                        if (encountered_item.name[1] == '1') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                    else
                    {
                        if (check_position == ex�eption_position_second) break;
                        if (encountered_item.name[0] == 'b') break;
                        if (encountered_item.name[1] == '1') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }

                }
                check_position.y += 2;
            }
            check_position.y = check_item_position.y - 2;
            while (check_position.y >= -6)
            {
                encountered_item = GetItemAt(check_position);
                if ((encountered_item != null && check_position != exception_position_first) || check_position == ex�eption_position_second)
                {
                    if (!move_tern)
                    {
                        if (check_position == ex�eption_position_second) break;
                        if (encountered_item.name[0] == 'w') break;
                        if (encountered_item.name[1] == '1') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                    else
                    {
                        if (check_position == ex�eption_position_second) break;
                        if (encountered_item.name[0] == 'b') break;
                        if (encountered_item.name[1] == '1') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }

                }
                check_position.y -= 2;
            }
            check_position.y = check_item_position.y;
            check_position.x = check_item_position.x + 2;
            while (check_position.x <= 7)
            {
                encountered_item = GetItemAt(check_position);
                if ((encountered_item != null && check_position != exception_position_first) || check_position == ex�eption_position_second)
                {
                    if (!move_tern)
                    {
                        if (check_position == ex�eption_position_second) break;
                        if (encountered_item.name[0] == 'w') break;
                        if (encountered_item.name[1] == '1') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                    else
                    {
                        if (check_position == ex�eption_position_second) break;
                        if (encountered_item.name[0] == 'b') break;
                        if (encountered_item.name[1] == '1') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }

                }
                check_position.x += 2;
            }
            check_position.x = check_item_position.x - 2;
            while (check_position.x >= -7)
            {
                encountered_item = GetItemAt(check_position);
                if ((encountered_item != null && check_position != exception_position_first) || check_position == ex�eption_position_second)
                {
                    if (!move_tern)
                    {
                        if (check_position == ex�eption_position_second) break;
                        if (encountered_item.name[0] == 'w') break;
                        if (encountered_item.name[1] == '1') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                    else
                    {
                        if (check_position == ex�eption_position_second) break;
                        if (encountered_item.name[0] == 'b') break;
                        if (encountered_item.name[1] == '1') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }

                }
                check_position.x -= 2;
            }
            check_position.x = check_item_position.x + 2;
            check_position.y = check_item_position.y + 2;
            while (check_position.y <= 8 && check_position.x <= 7)
            {
                encountered_item = GetItemAt(check_position);
                if ((encountered_item != null && check_position != exception_position_first) || check_position == ex�eption_position_second)
                {
                    if (!move_tern)
                    {
                        if (check_position == ex�eption_position_second) break;
                        if (encountered_item.name[0] == 'w') break;
                        if (encountered_item.name[1] == '2') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                    else
                    {
                        if (check_position == ex�eption_position_second) break;
                        if (encountered_item.name[0] == 'b') break;
                        if (encountered_item.name[1] == '2') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                }
                check_position.x += 2;
                check_position.y += 2;
            }
            check_position.x = check_item_position.x - 2;
            check_position.y = check_item_position.y - 2;
            while (check_position.y >= -6 && check_position.x >= -7)
            {
                encountered_item = GetItemAt(check_position);
                if ((encountered_item != null && check_position != exception_position_first) || check_position == ex�eption_position_second)
                {
                    if (!move_tern)
                    {
                        if (check_position == ex�eption_position_second) break;
                        if (encountered_item.name[0] == 'w') break;
                        if (encountered_item.name[1] == '2') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                    else
                    {
                        if (check_position == ex�eption_position_second) break;
                        if (encountered_item.name[0] == 'b') break;
                        if (encountered_item.name[1] == '2') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                }
                check_position.x -= 2;
                check_position.y -= 2;
            }
            check_position.x = check_item_position.x + 2;
            check_position.y = check_item_position.y - 2;
            while (check_position.y >= -6 && check_position.x <= 7)
            {
                encountered_item = GetItemAt(check_position);
                if ((encountered_item != null && check_position != exception_position_first) || check_position == ex�eption_position_second)
                {
                    if (!move_tern)
                    {
                        if (check_position == ex�eption_position_second) break;
                        if (encountered_item.name[0] == 'w') break;
                        if (encountered_item.name[1] == '2') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                    else
                    {
                        if (check_position == ex�eption_position_second) break;
                        if (encountered_item.name[0] == 'b') break;
                        if (encountered_item.name[1] == '2') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                }
                check_position.x += 2;
                check_position.y -= 2;
            }
            check_position.x = check_item_position.x - 2;
            check_position.y = check_item_position.y + 2;
            while (check_position.y <= 8 && check_position.x >= -7)
            {
                encountered_item = GetItemAt(check_position);
                if ((encountered_item != null && check_position != exception_position_first) || check_position == ex�eption_position_second)
                {
                    if (!move_tern)
                    {
                        if (check_position == ex�eption_position_second) break;
                        if (encountered_item.name[0] == 'w') break;
                        if (encountered_item.name[1] == '2') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                    else
                    {
                        if (check_position == ex�eption_position_second) break;
                        if (encountered_item.name[0] == 'b') break;
                        if (encountered_item.name[1] == '2') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                }
                check_position.x -= 2;
                check_position.y += 2;
            }
            if (!move_tern)
            {
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 2, check_item_position.y + 4));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '3' && !(encountered_item.transform.position.x==ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 4, check_item_position.y + 2));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '3' && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 2, check_item_position.y - 4));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '3' && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 4, check_item_position.y - 2));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '3' && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 2, check_item_position.y + 4));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '3' && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 4, check_item_position.y + 2));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '3' && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 2, check_item_position.y - 4));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '3' && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 4, check_item_position.y - 2));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '3' && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 2, check_item_position.y + 2));
                if (encountered_item != null && encountered_item.name[0] == 'b' && (encountered_item.name[1] == '4' || encountered_item.name[1] == '5') && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 2, check_item_position.y + 2));
                if (encountered_item != null && encountered_item.name[0] == 'b' && (encountered_item.name[1] == '4' || encountered_item.name[1] == '5') && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x, check_item_position.y + 2));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '5' && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 2, check_item_position.y));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '5' && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 2, check_item_position.y));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '5' && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 2, check_item_position.y - 2));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '5' && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 2, check_item_position.y - 2));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '5' && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x, check_item_position.y - 2));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '5' && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;

            }
            else
            {
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 2, check_item_position.y + 4));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '3' && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 4, check_item_position.y + 2));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '3' && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 2, check_item_position.y - 4));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '3' && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 4, check_item_position.y - 2));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '3' && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 2, check_item_position.y + 4));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '3' && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 4, check_item_position.y + 2));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '3' && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 2, check_item_position.y - 4));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '3' && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 4, check_item_position.y - 2));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '3' && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 2, check_item_position.y - 2));
                if (encountered_item != null && encountered_item.name[0] == 'w' && (encountered_item.name[1] == '4' || encountered_item.name[1] == '5') && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 2, check_item_position.y - 2));
                if (encountered_item != null && encountered_item.name[0] == 'w' && (encountered_item.name[1] == '4' || encountered_item.name[1] == '5') && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x, check_item_position.y - 2));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '5' && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 2, check_item_position.y));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '5' && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 2, check_item_position.y));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '5' && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 2, check_item_position.y + 2));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '5' && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 2, check_item_position.y + 2));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '5' && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x, check_item_position.y + 2));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '5' && !(encountered_item.transform.position.x == ex�eption_position_second.x && encountered_item.transform.position.y == ex�eption_position_second.y)) return true;

            }
            return false;
        }


        /// <summary>
        /// ��������� ����� �� ����� ������ ���������� ���� ������ � ��������� �������
        /// </summary>
        /// <param name="check_item_position">������� ���� ������������ ���</param>
        /// <param name="exception_item">������ ������� ����� </param>
        /// <returns></returns>
        bool checkAtack(Vector2 check_item_position, GameObject exception_item)
        {
            Vector2 check_position = new Vector2(check_item_position.x, check_item_position.y + 2);
            Transform encountered_item;
            while (check_position.y <= 8)
            {
                encountered_item = GetItemAt(check_position);
                if (encountered_item != null && encountered_item.gameObject!=exception_item)
                {
                    if (!move_tern)
                    {
                        if (encountered_item.name[0] == 'w') break;
                        if (encountered_item.name[1] == '1') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                    else
                    {
                        if (encountered_item.name[0] == 'b') break;
                        if (encountered_item.name[1] == '1') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }

                }
                check_position.y += 2;
            }
            check_position.y = check_item_position.y - 2;
            while (check_position.y >= -6)
            {
                encountered_item = GetItemAt(check_position);
                if (encountered_item != null && encountered_item.gameObject != exception_item)
                {
                    if (!move_tern)
                    {
                        if (encountered_item.name[0] == 'w') break;
                        if (encountered_item.name[1] == '1') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                    else
                    {
                        if (encountered_item.name[0] == 'b') break;
                        if (encountered_item.name[1] == '1') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }

                }
                check_position.y -= 2;
            }
            check_position.y = check_item_position.y;
            check_position.x = check_item_position.x + 2;
            while (check_position.x <= 7)
            {
                encountered_item = GetItemAt(check_position);
                if (encountered_item != null && encountered_item.gameObject != exception_item)
                {
                    if (!move_tern)
                    {
                        if (encountered_item.name[0] == 'w') break;
                        if (encountered_item.name[1] == '1') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                    else
                    {
                        if (encountered_item.name[0] == 'b') break;
                        if (encountered_item.name[1] == '1') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }

                }
                check_position.x += 2;
            }
            check_position.x = check_item_position.x - 2;
            while (check_position.x >= -7)
            {
                encountered_item = GetItemAt(check_position);
                if (encountered_item != null && encountered_item.gameObject != exception_item)
                {
                    if (!move_tern)
                    {
                        if (encountered_item.name[0] == 'w') break;
                        if (encountered_item.name[1] == '1') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                    else
                    {
                        if (encountered_item.name[0] == 'b') break;
                        if (encountered_item.name[1] == '1') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }

                }
                check_position.x -= 2;
            }
            check_position.x = check_item_position.x + 2;
            check_position.y = check_item_position.y + 2;
            while (check_position.y <= 8 && check_position.x <= 7)
            {
                encountered_item = GetItemAt(check_position);
                if (encountered_item != null && encountered_item.gameObject != exception_item)
                {
                    if (!move_tern)
                    {
                        if (encountered_item.name[0] == 'w') break;
                        if (encountered_item.name[1] == '2') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                    else
                    {
                        if (encountered_item.name[0] == 'b') break;
                        if (encountered_item.name[1] == '2') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                }
                check_position.x += 2;
                check_position.y += 2;
            }
            check_position.x = check_item_position.x - 2;
            check_position.y = check_item_position.y - 2;
            while (check_position.y >= -6 && check_position.x >= -7)
            {
                encountered_item = GetItemAt(check_position);
                if (encountered_item != null && encountered_item.gameObject != exception_item)
                {
                    if (!move_tern)
                    {
                        if (encountered_item.name[0] == 'w') break;
                        if (encountered_item.name[1] == '2') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                    else
                    {
                        if (encountered_item.name[0] == 'b') break;
                        if (encountered_item.name[1] == '2') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                }
                check_position.x -= 2;
                check_position.y -= 2;
            }
            check_position.x = check_item_position.x + 2;
            check_position.y = check_item_position.y - 2;
            while (check_position.y >= -6 && check_position.x <= 7)
            {
                encountered_item = GetItemAt(check_position);
                if (encountered_item != null && encountered_item.gameObject != exception_item)
                {
                    if (!move_tern)
                    {
                        if (encountered_item.name[0] == 'w') break;
                        if (encountered_item.name[1] == '2') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                    else
                    {
                        if (encountered_item.name[0] == 'b') break;
                        if (encountered_item.name[1] == '2') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                }
                check_position.x += 2;
                check_position.y -= 2;
            }
            check_position.x = check_item_position.x - 2;
            check_position.y = check_item_position.y + 2;
            while (check_position.y <= 8 && check_position.x >= -7)
            {
                encountered_item = GetItemAt(check_position);
                if (encountered_item != null && encountered_item.gameObject != exception_item)
                {
                    if (!move_tern)
                    {
                        if (encountered_item.name[0] == 'w') break;
                        if (encountered_item.name[1] == '2') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                    else
                    {
                        if (encountered_item.name[0] == 'b') break;
                        if (encountered_item.name[1] == '2') return true;
                        if (encountered_item.name[1] == '6') return true;
                        break;
                    }
                }
                check_position.x -= 2;
                check_position.y += 2;
            }
            if (!move_tern)
            {
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 2, check_item_position.y + 4));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 4, check_item_position.y + 2));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 2, check_item_position.y - 4));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 4, check_item_position.y - 2));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 2, check_item_position.y + 4));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 4, check_item_position.y + 2));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 2, check_item_position.y - 4));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 4, check_item_position.y - 2));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 2, check_item_position.y + 2));
                if (encountered_item != null && encountered_item.name[0] == 'b' && (encountered_item.name[1] == '4' || encountered_item.name[1] == '5')) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 2, check_item_position.y + 2));
                if (encountered_item != null && encountered_item.name[0] == 'b' && (encountered_item.name[1] == '4' || encountered_item.name[1] == '5')) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x, check_item_position.y + 2));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '5') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 2, check_item_position.y));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '5') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 2, check_item_position.y));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '5') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x, check_item_position.y - 2));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '5') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 2, check_item_position.y - 2));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '5') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 2, check_item_position.y - 2));
                if (encountered_item != null && encountered_item.name[0] == 'b' && encountered_item.name[1] == '5') return true;
            }
            else
            {
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 2, check_item_position.y + 4));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 4, check_item_position.y + 2));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 2, check_item_position.y - 4));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 4, check_item_position.y - 2));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 2, check_item_position.y + 4));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 4, check_item_position.y + 2));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 2, check_item_position.y - 4));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 4, check_item_position.y - 2));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '3') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 2, check_item_position.y - 2));
                if (encountered_item != null && encountered_item.name[0] == 'w' && (encountered_item.name[1] == '4' || encountered_item.name[1] == '5')) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 2, check_item_position.y - 2));
                if (encountered_item != null && encountered_item.name[0] == 'w' && (encountered_item.name[1] == '4' || encountered_item.name[1] == '5')) return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x, check_item_position.y - 2));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '5') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 2, check_item_position.y));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '5') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 2, check_item_position.y));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '5') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x + 2, check_item_position.y + 2));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '5') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x - 2, check_item_position.y + 2));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '5') return true;
                encountered_item = GetItemAt(new Vector2(check_item_position.x, check_item_position.y + 2));
                if (encountered_item != null && encountered_item.name[0] == 'w' && encountered_item.name[1] == '5') return true;
            }
            return false;
        }


        /// <summary>
        /// ��������� ��������� ������ � �� ��������� ��������
        /// </summary>
        /// <param name="pointer">��������� ������ </param>
        /// <returns></returns>
        string transformPointerToPositionName(Vector3 pointer)
        {
            pointer.y = (pointer.y + 6) / 2 + 49;
            pointer.x = (pointer.x + 7) / 2 + 97;
            return char.ConvertFromUtf32(((int)pointer.x)) + char.ConvertFromUtf32(((int)pointer.y));
        }


        /// <summary>
        /// ������� ����������� ������ �� ���
        /// </summary>
        /// <returns></returns>
        bool isMouseActionPresed() 
        {
            return Input.GetMouseButton(0);
        }

        /// <summary>
        /// ��������� ����
        /// none - ������ ���� (�� ������� ����� ����� � ����)
        /// waiting - ������������� ��������� ������� ���� ���� ���� ��������
        /// drag - ��������� ����� ������� ����� ��� ������� ��� �������� ��� ��� ���������� ������ ������
        /// </summary>
        enum State
        {
            none,
            waiting,
            drag
        }

    }
}
