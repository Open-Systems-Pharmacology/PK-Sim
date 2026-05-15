[PK SHINY](https://yourshinyappurl.com)
library(shiny)
library(nlmixr2)
library(rxode2)
library(ggplot2)

# =========================
# MODEL DEFINITIONS
# =========================

mtx_model <- function() {
  ini({
    tcl <- log(6)
    tvc <- log(20)
    tvp <- log(10)
    tq  <- log(0.5)

    eta.cl ~ 0.1
    eta.vc ~ 0.1

    add.err <- 0.5
    prop.err <- 0.2
  })

  model({
    CL <- exp(tcl + eta.cl) * (WT/70)^0.75
    Vc <- exp(tvc + eta.vc) * (WT/70)
    Vp <- exp(tvp)          * (WT/70)
    Q  <- exp(tq)           * (WT/70)^0.75

    d/dt(central) = -(CL/Vc)*central - (Q/Vc)*central + (Q/Vp)*periph
    d/dt(periph)  =  (Q/Vc)*central - (Q/Vp)*periph

    cp = central / Vc

    cp ~ prop(prop.err) + add(add.err)
  })
}

mtx_rx <- rxode2({
  CL = exp(tcl + eta.cl) * (WT/70)^0.75
  Vc = exp(tvc + eta.vc) * (WT/70)
  Vp = exp(tvp)          * (WT/70)
  Q  = exp(tq)           * (WT/70)^0.75

  d/dt(central) = -(CL/Vc)*central - (Q/Vc)*central + (Q/Vp)*periph
  d/dt(periph)  =  (Q/Vc)*central - (Q/Vp)*periph

  cp = central / Vc
})

theta <- c(
  tcl = 1.7917595,
  tvc = 2.9957323,
  tvp = 2.3025851,
  tq  = -0.6931472
)

# =========================
# UI
# =========================
ui <- fluidPage(
  titlePanel("Methotrexate Bayesian TDM Clinical Tool"),

  sidebarLayout(
    sidebarPanel(
      h4("Patient Inputs"),

      numericInput("wt", "Weight (kg)", 20),
      numericInput("dose", "Dose (mg)", 3000),
      numericInput("inf_dur", "Infusion Duration (h)", 36),

      h4("Observed Data"),
      numericInput("time1", "Sampling Time (h)", 42),
      numericInput("conc1", "MTX Concentration (mg/L)", 0.5),

      actionButton("run", "Run Bayesian Prediction")
    ),

    mainPanel(
      plotOutput("pkplot", height = "400px"),
      verbatimTextOutput("params"),
      verbatimTextOutput("prediction"),
      verbatimTextOutput("clinical")
    )
  )
)

# =========================
# SERVER
# =========================
server <- function(input, output) {

  observeEvent(input$run, {

    # -------------------------
    # Create patient dataset
    # -------------------------
    new_data <- data.frame(
      ID = 1,
      TIME = c(0, input$time1),
      AMT = c(input$dose, NA),
      DV  = c(NA, input$conc1),
      EVID = c(1, 0),
      WT = input$wt,
      CMT = 1
    )

    # -------------------------
    # Bayesian estimation
    # -------------------------
    fit <- nlmixr2(mtx_model, new_data, est="posthoc")

    eta <- fit$eta

    params <- c(
      theta,
      eta.cl = eta$eta.cl,
      eta.vc = eta$eta.vc,
      WT = input$wt
    )

    # -------------------------
    # Simulation dataset
    # -------------------------
    sim_data <- data.frame(
      time = seq(0, 100, by=1),
      WT = input$wt,
      amt = 0,
      rate = 0,
      evid = 0,
      cmt = 1
    )

    sim_data$amt[1]  <- input$dose
    sim_data$rate[1] <- input$dose / input$inf_dur
    sim_data$evid[1] <- 1

    sim <- rxSolve(mtx_rx, params, sim_data)

    # -------------------------
    # Plot
    # -------------------------
    output$pkplot <- renderPlot({
      ggplot() +
        geom_line(data = sim, aes(time, cp), color="blue", size=1) +
        geom_point(aes(input$time1, input$conc1),
                   color="red", size=4) +
        scale_y_log10() +
        theme_bw() +
        labs(
          title = "Predicted MTX PK Profile",
          x = "Time (h)",
          y = "Concentration (mg/L)"
        )
    })

    # -------------------------
    # Extract CL
    # -------------------------
    CL <- exp(theta["tcl"] + eta$eta.cl) * (input$wt/70)^0.75

    output$params <- renderPrint({
      cat("Estimated Clearance (CL):", round(CL,2), "L/h\n")
    })

    # -------------------------
    # Predict 48h level
    # -------------------------
    pred48 <- sim$cp[which(sim$time == 48)]

    output$prediction <- renderPrint({
      cat("Predicted 48h MTX:", round(pred48,3), "mg/L\n")
    })

    # -------------------------
    # Clinical interpretation
    # -------------------------
    output$clinical <- renderPrint({

      if(pred48 > 1){
        cat("⚠ HIGH RISK: Delayed clearance\nConsider aggressive leucovorin rescue\n")
      } else if(pred48 > 0.1){
        cat("⚠ Moderate risk: monitor closely\n")
      } else {
        cat("✓ Normal elimination\n")
      }

    })

  })
}

# =========================
# RUN APP
# =========================
shinyApp(ui, server)
