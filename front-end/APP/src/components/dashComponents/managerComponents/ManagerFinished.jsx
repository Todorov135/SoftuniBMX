import styles from "./ManagerFinished.module.css";

import { useEffect, useState } from "react";

import BoardHeader from "../BoardHeader.jsx";
import LoaderWheel from "../../LoaderWheel.jsx";

import { get } from "../../../util/api.js";
import { environment } from "../../../environments/environment.js";

import FinishedOrderFullElement from "./FinishedOrderFullElement.jsx";
import FinishedOrder from "./FinishedOrder.jsx";
import Popup from "../../Popup.jsx";

function ManagerFinished() {
  const [background, setBackground] = useState(false);
  const [currentOrder, setCurrentOrder] = useState({});

  const [error, setError] = useState({});
  const [loading, setLoading] = useState(false);
  const [orderList, setOrderList] = useState([]);

  // State to hold user input
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");

  const abortController = new AbortController();

  async function getFinishedOrders(e) {
    e.preventDefault();
    setLoading(true);
    const queryString = `?startDate=${startDate}&endDate=${endDate}`;
    const result = await get(environment.finished_orders + queryString);
    if (!result) {
      setLoading(false);
      return setError({
        message: "Something went wrong. Service can not get data!",
      });
    }
    setOrderList(result);
    setLoading(false);

    if (orderList.length === 0)
      return <h2>There is no orders in this category</h2>;
  }

  function onOrderButtonClick(o) {
    setCurrentOrder(o);
    setBackground(true);
  }

  function close(e) {
    setCurrentOrder({});
    setBackground(false);
  }

  return (
    <>
      <section className={styles.board}>
        <BoardHeader />
        {loading && <LoaderWheel />}

        <h2 className={styles.boardHeading}>Select time period:</h2>

        <section className={styles.section}>
          <form className={styles.form}>
            <label className={styles.label}>
              Start Date:
              <input
                className={styles.input}
                type="date"
                value={startDate}
                onChange={(e) => setStartDate(e.target.value)}
              />
            </label>
            <label className={styles.label}>
              End Date:
              <input
                className={styles.input}
                type="date"
                value={endDate}
                onChange={(e) => setEndDate(e.target.value)}
              />
            </label>
            <button className={styles.btnAdd} onClick={getFinishedOrders}>
              Get Orders
            </button>
          </form>
        </section>
      </section>

      {background && (
        <Popup onClose={close}>
          <FinishedOrderFullElement order={currentOrder} />
        </Popup>
      )}
      <h2 className={styles.dashHeading}>
        Orders in sequence by time of creation
      </h2>
      <section className={styles.board}>
        <div className={styles.orders}>
          {loading && <LoaderWheel />}
          {orderList.map((order, i) => (
            <FinishedOrder
              key={order.orderId}
              order={order}
              i={i + 1}
              onOrderButtonClick={onOrderButtonClick}
            />
          ))}
        </div>
      </section>
    </>
  );
}

export default ManagerFinished;
