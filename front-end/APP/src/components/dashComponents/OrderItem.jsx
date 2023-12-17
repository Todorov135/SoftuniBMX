import styles from "./OrderItem.module.css";

import { useContext, useEffect, useState } from "react";
import { secondsToTime } from "../../util/util.js";
import { UserContext } from "../../context/GlobalUserProvider.jsx";
import { useNavigate } from "react-router-dom";
import { post } from "../../util/api.js";
import { environment } from "../../environments/environment.js";
import LoaderWheel from "../LoaderWheel.jsx";

function OrderItem({ product, onBtnHandler, orderIndex }) {
  const { user } = useContext(UserContext);
  const [loading, setLoading] = useState(false);

  async function onButtonClick() {
    const currentDate = new Date().toISOString();
    const model = {
      partId: product.partId,
      employeeId: user.id,
      dateTime: currentDate,
    };
    let path = "";

    // console.log(product);
    // console.log(model);

    if (product.datetimeAsigned === null) {
      path = "start";
    } else if (product.datetimeFinished === null) {
      path = "end";
    }

    const result = await post(environment.worker_order + path, model);
    console.log(result);
    onBtnHandler();

    // if (item.startedTime === "" && item.finishedTime === "") {
    //   setItem({
    //     ...item,
    //     startedTime: currentDate,
    //     nameOfEmplоyeeProducedThePart: `${user.firstName} ${user.lastName}`,
    //   });
    //   setFirstCall(!firstCall);
    // } else if (item.startedTime !== "" && item.finishedTime === "") {
    //   setItem({ ...item, finishedTime: currentDate, isProduced: true });
    //   setIsDone(!isDone);
    // }
  }

  return (
    <>
      {loading && <LoaderWheel />}
      <figure className={styles.order}>
        <header className={styles.header}>
          <p className={styles.model}>
            <span>SN: </span>
            {product.orderSerialNumber}
          </p>
          <p className={styles.model}>
            <span>Date created: </span>
            {product.datetimeAsigned.split(" ").at(0).replaceAll("/", ".")}
          </p>
        </header>

        {/* <div className={styles.info}> */}
        <h3 className={styles.brand}>
          <span>Brand: </span>
          {product.partName}
        </h3>
        <p className={styles.model}>
          <span>OEM Number: </span>
          {product.partOEMNumber}
        </p>
        <div className={styles.model}>
          <span>Description:</span>
          {product.description}
        </div>
        {/* </div> */}
        {/* <div className={styles.info}> */}
        <p className={styles.model}>
          <span>Started on: </span>
          {product.datetimeAsigned &&
            product.datetimeAsigned.replaceAll("/", ".")}
        </p>
        <p className={`${styles.model} ${styles.shortLine}`}>
          <span>Finished on: </span>
          {product.datetimeFinished &&
            product.datetimeFinished.replaceAll("/", ".")}
        </p>
        {/* <p className={styles.partId}>ID# {item.partId}</p> */}
        <p className={styles.partId}>
          ID# {product.partId + "-" + product.partId}
        </p>
        {/* </div> */}

        <div className={styles.timer}>
          <p className={styles.prod}>
            <span>Produced by: </span>
            {`${user.firstName} ${user.lastName}`}
          </p>
          <button
            className={styles.startBtn}
            onClick={onButtonClick}
            disabled={orderIndex !== 0}
          >
            {/* {product.datetimeAsigned === "" &&
              product.datetimeFinished === "" &&
              orderIndex !== 0 &&
              "to Queue"}
            {product.datetimeAsigned === "" &&
              product.datetimeFinished === "" &&
              orderIndex === 0 &&
              "Start"}
            {product.datetimeAsigned !== "" &&
              product.datetimeFinished === "" &&
              "In Progress"}
            {product.datetimeAsigned !== "" &&
              product.datetimeFinished !== "" &&
              "Finished"} */}
            {orderIndex === 0 && !product.datetimeAsigned && "Start"}
            {orderIndex === 0 && product.datetimeAsigned && "In Progress"}
            {orderIndex !== 0 && "to Queue"}
          </button>
        </div>

        <img className={styles.background} src="/img/bg-bike.webp" alt="" />
      </figure>
    </>
  );
}

export default OrderItem;
